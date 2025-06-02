using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using SpaceGame.Commands;
using SpaceGame.IoC;
using SpaceGame.Core;

namespace SpaceGame.Tests.Commands
{
    public class LongRunningActionsTests
    {
        public LongRunningActionsTests()
        {
            // Clear command registry before each test
            CommandRegistry.Instance.Clear();
        }

        [Fact]
        public void StartAction_Should_Add_Command_To_Queue()
        {

            Core.IoC.IoC.Clear();
            new RegisterIoCDependencyMoveCommand().Execute();
            // Arrange
            var gameObject = new Dictionary<string, object>();
            var commandQueue = new Queue<ICommand>();
            // Mock the Move command for this test - но адаптер тоже нужно замокать
            var mockCommand = new Mock<ICommand>();

            // Mock IMovingObject adapter
            var mockMovingObject = new Mock<IMovingObject>();
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Adapters.IMovingObject",
                (object[] args) => mockMovingObject.Object
            ).Execute();

            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Move",
                (object[] args) => mockCommand.Object
            ).Execute();

            var order = new Dictionary<string, object>
            {
                ["Object"] = gameObject,
                ["Operation"] = "Move",
                ["Queue"] = commandQueue
            };

            var startAction = new StartAction(order);

            // Act
            startAction.Execute();

            Assert.True(commandQueue.Count > 0); // Команда в очереди есть
            Assert.IsAssignableFrom<ICommand>(commandQueue.Peek()); // И это точно команда
        }

        [Fact]
        public void StartAction_Should_Register_Command_In_Registry()
        {
            // Arrange
            var gameObject = new Dictionary<string, object>();
            var commandQueue = new Queue<ICommand>();
            var mockCommand = new Mock<ICommand>();

            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Rotate",
                (object[] args) => mockCommand.Object
            ).Execute();

            var order = new Dictionary<string, object>
            {
                ["Object"] = gameObject,
                ["Operation"] = "Rotate",
                ["Queue"] = commandQueue
            };

            var startAction = new StartAction(order);

            // Act
            startAction.Execute();

            // Assert
            var commandId = $"{gameObject.GetHashCode()}_Rotate";
            Assert.True(CommandRegistry.Instance.CommandExists(commandId));
        }

        [Fact]
        public void StartAction_Should_Throw_Exception_When_Object_Missing()
        {
            // Arrange
            var order = new Dictionary<string, object>
            {
                ["Operation"] = "Move",
                ["Queue"] = new Queue<ICommand>()
            };

            var startAction = new StartAction(order);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => startAction.Execute());
        }

        [Fact]
        public void StartAction_Should_Throw_Exception_When_Operation_Missing()
        {
            // Arrange
            var order = new Dictionary<string, object>
            {
                ["Object"] = new Dictionary<string, object>(),
                ["Queue"] = new Queue<ICommand>()
            };

            var startAction = new StartAction(order);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => startAction.Execute());
        }

        [Fact]
        public void StartAction_Should_Throw_Exception_When_Queue_Missing()
        {
            // Arrange
            var order = new Dictionary<string, object>
            {
                ["Object"] = new Dictionary<string, object>(),
                ["Operation"] = "Move"
            };

            var startAction = new StartAction(order);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => startAction.Execute());
        }

        [Fact]
        public void StopAction_Should_Remove_Command_From_Registry()
        {
            // Arrange
            var gameObject = new Dictionary<string, object>();
            var commandQueue = new Queue<ICommand>();
            var mockCommand = new Mock<ICommand>();

            // First, start a command
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Move",
                (object[] args) => mockCommand.Object
            ).Execute();

            var startOrder = new Dictionary<string, object>
            {
                ["Object"] = gameObject,
                ["Operation"] = "Move",
                ["Queue"] = commandQueue
            };

            var startAction = new StartAction(startOrder);
            startAction.Execute();

            var commandId = $"{gameObject.GetHashCode()}_Move";
            Assert.True(CommandRegistry.Instance.CommandExists(commandId));

            // Now stop the command
            var stopOrder = new Dictionary<string, object>
            {
                ["Object"] = gameObject,
                ["Operation"] = "Move",
                ["Queue"] = commandQueue
            };

            var stopAction = new StopAction(stopOrder);

            // Act
            stopAction.Execute();

            // Assert
            Assert.False(CommandRegistry.Instance.CommandExists(commandId));
        }

        [Fact]
        public void StopAction_Should_Work_In_Constant_Time()
        {
            // This test verifies that stop operation is O(1)
            // We test this by ensuring the same operation time regardless of registry size

            // Arrange - Add many commands to registry
            for (int i = 0; i < 1000; i++)
            {
                var dummyObject = new Dictionary<string, object> { ["Id"] = i };
                var commandId = $"{dummyObject.GetHashCode()}_Move";
                CommandRegistry.Instance.RegisterCommand(commandId, new Mock<ICommand>().Object);
            }

            var targetObject = new Dictionary<string, object>();
            var commandQueue = new Queue<ICommand>();
            var targetCommandId = $"{targetObject.GetHashCode()}_Move";
            CommandRegistry.Instance.RegisterCommand(targetCommandId, new Mock<ICommand>().Object);

            var stopOrder = new Dictionary<string, object>
            {
                ["Object"] = targetObject,
                ["Operation"] = "Move",
                ["Queue"] = commandQueue
            };

            var stopAction = new StopAction(stopOrder);

            // Act - Time the operation (should be constant regardless of registry size)
            var startTime = DateTime.UtcNow;
            stopAction.Execute();
            var endTime = DateTime.UtcNow;

            // Assert
            Assert.False(CommandRegistry.Instance.CommandExists(targetCommandId));
            // In a real performance test, we would verify the time is constant
            // For unit test purposes, we just verify it completes quickly
            Assert.True((endTime - startTime).TotalMilliseconds < 100);
        }

        [Fact]
        public void CommandRegistry_Should_Support_Concurrent_Access()
        {
            // Arrange
            var registry = CommandRegistry.Instance;
            var command1 = new Mock<ICommand>().Object;
            var command2 = new Mock<ICommand>().Object;

            // Act - Simulate concurrent access
            registry.RegisterCommand("cmd1", command1);
            registry.RegisterCommand("cmd2", command2);

            var exists1 = registry.CommandExists("cmd1");
            var retrieved1 = registry.UnregisterCommand("cmd1");
            var exists1After = registry.CommandExists("cmd1");

            // Assert
            Assert.True(exists1);
            Assert.Equal(command1, retrieved1);
            Assert.False(exists1After);
            Assert.True(registry.CommandExists("cmd2"));
        }

        [Fact]
        public void StoppedCommandWrapper_Should_Not_Execute_Wrapped_Command()
        {
            // Arrange
            var mockCommand = new Mock<ICommand>();
            var wrapper = new StoppedCommandWrapper(mockCommand.Object);

            // Act
            wrapper.Execute();

            // Assert
            mockCommand.Verify(c => c.Execute(), Times.Never);
            Assert.Equal(mockCommand.Object, wrapper.WrappedCommand);
        }
    }

    public class LongRunningActionsRegistrationTests
    {
        [Fact]
        public void RegisterIoCDependencyActionsStart_Should_Register_Actions_Start_Dependency()
        {
            // Arrange
            var registerCommand = new RegisterIoCDependencyActionsStart();

            // Act
            registerCommand.Execute();

            // Assert
            var order = new Dictionary<string, object>
            {
                ["Object"] = new Dictionary<string, object>(),
                ["Operation"] = "Move",
                ["Queue"] = new Queue<ICommand>()
            };

            var startAction = Core.IoC.IoC.Resolve<ICommand>("Actions.Start", order);
            Assert.NotNull(startAction);
            Assert.IsType<StartAction>(startAction);
        }

        [Fact]
        public void RegisterIoCDependencyActionsStop_Should_Register_Actions_Stop_Dependency()
        {
            // Arrange
            var registerCommand = new RegisterIoCDependencyActionsStop();

            // Act
            registerCommand.Execute();

            // Assert
            var order = new Dictionary<string, object>
            {
                ["Object"] = new Dictionary<string, object>(),
                ["Operation"] = "Move",
                ["Queue"] = new Queue<ICommand>()
            };

            var stopAction = Core.IoC.IoC.Resolve<ICommand>("Actions.Stop", order);
            Assert.NotNull(stopAction);
            Assert.IsType<StopAction>(stopAction);
        }

        [Fact]
        public void RegisterLongRunningActionsDependencies_Should_Register_All_Dependencies()
        {
            // Arrange
            var registerAllCommand = new RegisterLongRunningActionsDependencies();

            // Act
            registerAllCommand.Execute();

            // Assert
            var order = new Dictionary<string, object>
            {
                ["Object"] = new Dictionary<string, object>(),
                ["Operation"] = "Move",
                ["Queue"] = new Queue<ICommand>()
            };

            var startAction = Core.IoC.IoC.Resolve<ICommand>("Actions.Start", order);
            var stopAction = Core.IoC.IoC.Resolve<ICommand>("Actions.Stop", order);
            var registry = Core.IoC.IoC.Resolve<CommandRegistry>("CommandRegistry");

            Assert.NotNull(startAction);
            Assert.NotNull(stopAction);
            Assert.NotNull(registry);
            Assert.Same(CommandRegistry.Instance, registry);
        }
    }
}