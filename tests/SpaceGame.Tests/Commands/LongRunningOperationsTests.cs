using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using SpaceGame.Commands;
using SpaceGame.IoC;

namespace SpaceGame.Tests.Commands
{
    public class LongRunningOperationRegistryTests
    {
        [Fact]
        public void StartOperation_Should_Add_Operation_To_Registry()
        {
            // Arrange
            var registry = new LongRunningOperationRegistry();
            var mockCommand = new Mock<ICommand>();

            // Act
            registry.StartOperation("test-op", mockCommand.Object);

            // Assert
            Assert.True(registry.IsOperationActive("test-op"));
            Assert.Contains("test-op", registry.GetActiveOperationIds());
        }

        [Fact]
        public void StopOperation_Should_Remove_Operation_From_Registry()
        {
            // Arrange
            var registry = new LongRunningOperationRegistry();
            var mockCommand = new Mock<ICommand>();
            registry.StartOperation("test-op", mockCommand.Object);

            // Act
            var result = registry.StopOperation("test-op");

            // Assert
            Assert.True(result);
            Assert.False(registry.IsOperationActive("test-op"));
            Assert.DoesNotContain("test-op", registry.GetActiveOperationIds());
        }

        [Fact]
        public void StopOperation_Should_Return_False_For_Non_Existent_Operation()
        {
            // Arrange
            var registry = new LongRunningOperationRegistry();

            // Act
            var result = registry.StopOperation("non-existent");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void StartOperation_Should_Throw_Exception_For_Duplicate_Operation_Id()
        {
            // Arrange
            var registry = new LongRunningOperationRegistry();
            var mockCommand1 = new Mock<ICommand>();
            var mockCommand2 = new Mock<ICommand>();
            
            registry.StartOperation("duplicate-op", mockCommand1.Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                registry.StartOperation("duplicate-op", mockCommand2.Object));
        }

        [Fact]
        public void StopOperation_Should_Execute_In_Constant_Time()
        {
            // Arrange
            var registry = new LongRunningOperationRegistry();
            var mockCommand = new Mock<ICommand>();

            // Start multiple operations to ensure constant time
            for (int i = 0; i < 1000; i++)
            {
                registry.StartOperation($"op-{i}", mockCommand.Object);
            }

            // Act - stopping should be O(1) regardless of number of operations
            var startTime = DateTime.UtcNow;
            var result = registry.StopOperation("op-500");
            var endTime = DateTime.UtcNow;

            // Assert
            Assert.True(result);
            Assert.True((endTime - startTime).TotalMilliseconds < 100); // Should be very fast
        }
    }

    public class StartLongRunningOperationCommandTests
    {
        [Fact]
        public void StartLongRunningOperationCommand_Should_Start_Operation()
        {
            // Arrange
            var registry = new LongRunningOperationRegistry();
            var gameObject = new Dictionary<string, object>();
            var mockCommand = new Mock<ICommand>();

            // Mock IoC to return the command
            Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Commands.TestOperation",
                (object[] args) => mockCommand.Object
            ).Execute();

            var startCommand = new StartLongRunningOperationCommand(
                "test-op", "TestOperation", gameObject, registry);

            // Act
            startCommand.Execute();

            // Assert
            Assert.True(registry.IsOperationActive("test-op"));
        }

        [Fact]
        public void StartLongRunningOperationCommand_Should_Throw_Exception_For_Null_Parameters()
        {
            var registry = new LongRunningOperationRegistry();
            var gameObject = new Dictionary<string, object>();

            // Test null operationId
            Assert.Throws<ArgumentNullException>(() => 
                new StartLongRunningOperationCommand(null, "Move", gameObject, registry));

            // Test null operationType
            Assert.Throws<ArgumentNullException>(() => 
                new StartLongRunningOperationCommand("op1", null, gameObject, registry));

            // Test null gameObject
            Assert.Throws<ArgumentNullException>(() => 
                new StartLongRunningOperationCommand("op1", "Move", null, registry));

            // Test null registry
            Assert.Throws<ArgumentNullException>(() => 
                new StartLongRunningOperationCommand("op1", "Move", gameObject, null));
        }
    }

    public class StopLongRunningOperationCommandTests
    {
        [Fact]
        public void StopLongRunningOperationCommand_Should_Stop_Running_Operation()
        {
            // Arrange
            var registry = new LongRunningOperationRegistry();
            var mockCommand = new Mock<ICommand>();
            
            registry.StartOperation("test-op", mockCommand.Object);
            var stopCommand = new StopLongRunningOperationCommand("test-op", registry);

            // Act
            stopCommand.Execute();

            // Assert
            Assert.False(registry.IsOperationActive("test-op"));
        }

        [Fact]
        public void StopLongRunningOperationCommand_Should_Throw_Exception_For_Non_Running_Operation()
        {
            // Arrange
            var registry = new LongRunningOperationRegistry();
            var stopCommand = new StopLongRunningOperationCommand("non-existent", registry);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => stopCommand.Execute());
        }
    }

    public class LongRunningOperationsRegistrationTests
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
                ["OperationId"] = "test-op",
                ["OperationType"] = "Move",
                ["GameObject"] = new Dictionary<string, object>()
            };

            var startCommand = Ioc.Resolve<ICommand>("Actions.Start", order);
            Assert.NotNull(startCommand);
            Assert.IsType<StartLongRunningOperationCommand>(startCommand);
        }

        [Fact]
        public void RegisterIoCDependencyActionsStop_Should_Register_Actions_Stop_Dependency()
        {
            // Arrange
            var registerStartCommand = new RegisterIoCDependencyActionsStart();
            var registerStopCommand = new RegisterIoCDependencyActionsStop();

            registerStartCommand.Execute();
            registerStopCommand.Execute();

            // Assert
            var order = new Dictionary<string, object>
            {
                ["OperationId"] = "test-op"
            };

            var stopCommand = Ioc.Resolve<ICommand>("Actions.Stop", order);
            Assert.NotNull(stopCommand);
            Assert.IsType<StopLongRunningOperationCommand>(stopCommand);
        }

        [Fact]
        public void Actions_Start_Should_Throw_Exception_With_Invalid_Order()
        {
            // Arrange
            var registerCommand = new RegisterIoCDependencyActionsStart();
            registerCommand.Execute();

            var invalidOrder = new Dictionary<string, object>
            {
                ["OperationId"] = "test-op"
                // Missing OperationType and GameObject
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Ioc.Resolve<ICommand>("Actions.Start", invalidOrder));
        }

        [Fact]
        public void Actions_Stop_Should_Throw_Exception_With_Invalid_Order()
        {
            // Arrange
            var registerStartCommand = new RegisterIoCDependencyActionsStart();
            var registerStopCommand = new RegisterIoCDependencyActionsStop();
            
            registerStartCommand.Execute();
            registerStopCommand.Execute();

            var invalidOrder = new Dictionary<string, object>
            {
                // Missing OperationId
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Ioc.Resolve<ICommand>("Actions.Stop", invalidOrder));
        }
    }
}