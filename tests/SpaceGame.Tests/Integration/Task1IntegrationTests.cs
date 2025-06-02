using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using SpaceGame.Commands;
using SpaceGame.Core;
using SpaceGame.IoC;

namespace SpaceGame.Tests.Integration
{
 
    [CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
    public class NonParallelCollectionDefinition { }


    
    /// <summary>
    /// Integration tests to verify all 20 points of Task 1 work together
    /// </summary>
    [Collection("Non-Parallel Collection")]
    public class Task1IntegrationTests
    {
        public Task1IntegrationTests()
        {
            // Initialize all Task 1 dependencies before each test
            Core.IoC.IoC.Clear();
            new RegisterAllTask1Dependencies().Execute();
            CommandRegistry.Instance.Clear();
        }

        [Fact]
        public void Complete_Task1_Workflow_Should_Work_End_To_End()
        {
            new RegisterIoCDependencyMoveCommand().Execute();
            new RegisterIoCDependencyRotateCommand().Execute();
            // Arrange - Create a spaceship that can move and rotate
            var spaceship = new Dictionary<string, object>
            {
                ["Position"] = new Vector(10, 10),
                ["Velocity"] = new Vector(5, 3),
                ["Direction"] = new Angle(45, 360),
                ["AngularVelocity"] = new Angle(15, 360)
            };

            // Test Points 6 & 9: Move and Rotate commands work via IoC
            var movingObject = Core.IoC.IoC.Resolve<IMovingObject>("Adapters.IMovingObject", spaceship);
            var rotatingObject = Core.IoC.IoC.Resolve<IRotatingObject>("Adapters.IRotatingObject", spaceship);

            var moveCommand = Core.IoC.IoC.Resolve<ICommand>("Commands.Move", movingObject);
            var rotateCommand = Core.IoC.IoC.Resolve<ICommand>("Commands.Rotate", rotatingObject);

            // Verify individual commands work
            moveCommand.Execute();
            Assert.Equal(new Vector(15, 13), spaceship["Position"]);

            rotateCommand.Execute();
            Assert.Equal(new Angle(60, 360), spaceship["Direction"]);

            // Test Points 11 & 13: Macro commands work
            var commands = new ICommand[] { moveCommand, rotateCommand };
            var macroCommand = Core.IoC.IoC.Resolve<ICommand>("Commands.Macro", commands);
            
            // Reset position and direction
            spaceship["Position"] = new Vector(0, 0);
            spaceship["Direction"] = new Angle(0, 360);

            macroCommand.Execute();
            
            // Verify both commands executed
            Assert.Equal(new Vector(5, 3), spaceship["Position"]);
            Assert.Equal(new Angle(15, 360), spaceship["Direction"]);
        }

        [Fact]
        public void Send_Command_Workflow_Should_Work()
        {
            // Arrange - Test Point 15: Send command
            var receiverMock = new Mock<ICommandReceiver>();
            var testCommandMock = new Mock<ICommand>();

            var sendCommand = Core.IoC.IoC.Resolve<ICommand>("Commands.Send", 
                testCommandMock.Object, receiverMock.Object);

            // Act
            sendCommand.Execute();

            // Assert
            receiverMock.Verify(r => r.Receive(testCommandMock.Object), Times.Once);
        }

        [Fact]
        public void Command_Injectable_Workflow_Should_Work()
        {
            // Arrange - Test Point 18: CommandInjectable
            var injectable = Core.IoC.IoC.Resolve<ICommandInjectable>("Commands.CommandInjectable");
            var executable = Core.IoC.IoC.Resolve<ICommand>("Commands.CommandInjectable");

            var testCommandMock = new Mock<ICommand>();

            // Act
            injectable.Inject(testCommandMock.Object);
            executable.Execute();

            // Assert
            testCommandMock.Verify(c => c.Execute(), Times.Once);
        }

        [Fact]
        public void Long_Running_Actions_Workflow_Should_Work()
        {
            // Arrange - Test Points 19 & 20: Start and Stop actions
            var spaceship = new Dictionary<string, object>
            {
                ["Position"] = new Vector(0, 0),
                ["Velocity"] = new Vector(1, 1)
            };

            var commandQueue = new Queue<ICommand>();

            // Mock the Move command for this test
            var mockMoveCommand = new Mock<ICommand>();
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Move",
                (object[] args) => mockMoveCommand.Object
            ).Execute();

            var startOrder = new Dictionary<string, object>
            {
                ["Object"] = spaceship,
                ["Operation"] = "Move",
                ["Queue"] = commandQueue
            };

            var stopOrder = new Dictionary<string, object>
            {
                ["Object"] = spaceship,
                ["Operation"] = "Move",
                ["Queue"] = commandQueue
            };

            // Act - Start long-running operation
            var startAction = Core.IoC.IoC.Resolve<ICommand>("Actions.Start", startOrder);
            startAction.Execute();

            // Verify command was added to queue and registry
            Assert.Single(commandQueue);
            var commandId = $"{spaceship.GetHashCode()}_Move";
            Assert.True(CommandRegistry.Instance.CommandExists(commandId));

            // Stop the operation
            var stopAction = Core.IoC.IoC.Resolve<ICommand>("Actions.Stop", stopOrder);
            stopAction.Execute();

            // Assert - Command should be removed from registry (O(1) operation)
            Assert.False(CommandRegistry.Instance.CommandExists(commandId));
        }

        [Fact]
        public void Vector_Operations_Should_Work_Correctly()
        {
            // Test Point 4: Vector operations
            var vector1 = new Vector(1, -1, 2);
            var vector2 = new Vector(-1, 1, -2);
            var result = vector1 + vector2;

            Assert.Equal(new Vector(0, 0, 0), result);

            // Test vector equality
            var vector3 = new Vector(1, 2, 3);
            var vector4 = new Vector(1, 2, 3);
            
            Assert.True(vector3.Equals(vector4));
            Assert.True(vector3 == vector4);
            Assert.False(vector3 != vector4);
            Assert.Equal(vector3.GetHashCode(), vector4.GetHashCode());
        }

        [Fact]
        public void Angle_Operations_Should_Work_Correctly()
        {
            // Test Point 7: Angle operations
            var angle1 = new Angle(5, 360);
            var angle2 = new Angle(7, 360);
            var result = angle1 + angle2;

            Assert.Equal(new Angle(12, 360), result);

            // Test angle equality (15/360 and 375/360 should be equal)
            var angle3 = new Angle(15, 360);
            var angle4 = new Angle(375, 360);
            
            Assert.True(angle3.Equals(angle4));
            Assert.True(angle3 == angle4);
            Assert.False(angle3 != angle4);
        }

        [Fact]
        public void Move_Command_Should_Update_Position_Correctly()
        {
            new RegisterIoCDependencyMoveCommand().Execute();
            // Test Point 5: Move command behavior
            var gameObject = new Dictionary<string, object>
            {
                ["Position"] = new Vector(12, 5),
                ["Velocity"] = new Vector(-4, 1)
            };

            var movingObject = Core.IoC.IoC.Resolve<IMovingObject>("Adapters.IMovingObject", gameObject);
            var moveCommand = new MoveCommand(movingObject);

            moveCommand.Execute();

            Assert.Equal(new Vector(8, 6), gameObject["Position"]);
        }

        [Fact]
        public void Rotate_Command_Should_Update_Direction_Correctly()
        {
            // Test Point 8: Rotate command behavior
            var gameObject = new Dictionary<string, object>
            {
                ["Direction"] = new Angle(45, 360),
                ["AngularVelocity"] = new Angle(45, 360)
            };

            var rotatingObject = Core.IoC.IoC.Resolve<IRotatingObject>("Adapters.IRotatingObject", gameObject);
            var rotateCommand = new RotateCommand(rotatingObject);

            rotateCommand.Execute();

            Assert.Equal(new Angle(90, 360), gameObject["Direction"]);
        }

        [Fact]
        public void Macro_Command_Should_Execute_Without_Loops()
        {
            // Test Point 10: MacroCommand without loops (recursive implementation)
            var executionOrder = new List<string>();
            
            var command1 = new TestCommand("First", executionOrder);
            var command2 = new TestCommand("Second", executionOrder);
            var command3 = new TestCommand("Third", executionOrder);

            var macroCommand = new MacroCommand(new ICommand[] { command1, command2, command3 });
            macroCommand.Execute();

            Assert.Equal(new[] { "First", "Second", "Third" }, executionOrder);
        }

        [Fact]
        public void Macro_Command_Should_Stop_On_Exception()
        {
            // Test Point 10: MacroCommand exception handling
            var executionOrder = new List<string>();
            
            var command1 = new TestCommand("First", executionOrder);
            var failingCommand = new FailingTestCommand("Failing", executionOrder);
            var command3 = new TestCommand("Third", executionOrder);

            var macroCommand = new MacroCommand(new ICommand[] { command1, failingCommand, command3 });

            Assert.Throws<InvalidOperationException>(() => macroCommand.Execute());
            
            // Only first command should have executed
            Assert.Single(executionOrder);
            Assert.Equal("First", executionOrder[0]);
        }

        [Fact]
        public void Error_Handling_Should_Work_For_Invalid_Operations()
        {
            // Test various error conditions from the task requirements

            // Test missing position for move
            var invalidMoveObject = new Dictionary<string, object>
            {
                ["Velocity"] = new Vector(1, 1)
                // No Position
            };

            Assert.ThrowsAny<Exception>(() =>
            {
                var movingObject = Core.IoC.IoC.Resolve<IMovingObject>("Adapters.IMovingObject", invalidMoveObject);
                var moveCommand = new MoveCommand(movingObject);
                moveCommand.Execute();
            });

            // Test invalid vector addition
            var vector1 = new Vector(1, 2, 3);
            var vector2 = new Vector(1, 2); // Different dimensions

            Assert.Throws<ArgumentException>(() => vector1 + vector2);
        }

        /// <summary>
        /// Helper test command that tracks execution order
        /// </summary>
        private class TestCommand : ICommand
        {
            private readonly string _name;
            private readonly List<string> _executionOrder;

            public TestCommand(string name, List<string> executionOrder)
            {
                _name = name;
                _executionOrder = executionOrder;
            }

            public void Execute()
            {
                _executionOrder.Add(_name);
            }
        }

        /// <summary>
        /// Helper test command that always fails
        /// </summary>
        private class FailingTestCommand : ICommand
        {
            private readonly string _name;
            private readonly List<string> _executionOrder;

            public FailingTestCommand(string name, List<string> executionOrder)
            {
                _name = name;
                _executionOrder = executionOrder;
            }

            public void Execute()
            {
                throw new InvalidOperationException($"{_name} command failed");
            }
        }
    }
}