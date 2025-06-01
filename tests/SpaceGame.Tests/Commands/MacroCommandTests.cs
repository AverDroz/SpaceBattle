using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using SpaceGame.Commands;
using SpaceGame.Core;

namespace SpaceGame.Tests.Commands
{
    public class MacroCommandTests
    {
        [Fact]
        public void MacroCommand_Should_Execute_All_Commands_From_Array()
        {
            // Arrange
            var executionOrder = new List<string>();
            
            var command1 = new TestCommand("Command1", executionOrder);
            var command2 = new TestCommand("Command2", executionOrder);
            var command3 = new TestCommand("Command3", executionOrder);
            
            var commands = new ICommand[] { command1, command2, command3 };
            var macroCommand = new MacroCommand(commands);

            // Act
            macroCommand.Execute();

            // Assert
            Assert.Equal(3, executionOrder.Count);
            Assert.Equal("Command1", executionOrder[0]);
            Assert.Equal("Command2", executionOrder[1]);
            Assert.Equal("Command3", executionOrder[2]);
        }

        [Fact]
        public void MacroCommand_Should_Execute_Empty_Array_Without_Error()
        {
            // Arrange
            var emptyCommands = new ICommand[0];
            var macroCommand = new MacroCommand(emptyCommands);

            // Act & Assert - should not throw
            macroCommand.Execute();
        }

        [Fact]
        public void MacroCommand_Should_Throw_Exception_If_One_Command_Throws_And_Stop_Execution()
        {
            // Arrange
            var executionOrder = new List<string>();
            
            var command1 = new TestCommand("Command1", executionOrder);
            var failingCommand = new FailingCommand("FailingCommand", executionOrder);
            var command3 = new TestCommand("Command3", executionOrder);
            
            var commands = new ICommand[] { command1, failingCommand, command3 };
            var macroCommand = new MacroCommand(commands);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => macroCommand.Execute());
            
            // Verify that only the first command executed before the failure
            Assert.Single(executionOrder);
            Assert.Equal("Command1", executionOrder[0]);
            Assert.Equal("FailingCommand failed", exception.Message);
        }

        [Fact]
        public void MacroCommand_Should_Throw_Exception_When_Commands_Array_Is_Null()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MacroCommand(null));
        }

        [Fact]
        public void MacroCommand_Should_Throw_Exception_When_Command_In_Array_Is_Null()
        {
            // Arrange
            var commands = new ICommand[] { new TestCommand("Command1", new List<string>()), null };
            var macroCommand = new MacroCommand(commands);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => macroCommand.Execute());
        }

        [Fact]
        public void MacroCommand_Should_Execute_Single_Command()
        {
            // Arrange
            var executionOrder = new List<string>();
            var singleCommand = new TestCommand("SingleCommand", executionOrder);
            var commands = new ICommand[] { singleCommand };
            var macroCommand = new MacroCommand(commands);

            // Act
            macroCommand.Execute();

            // Assert
            Assert.Single(executionOrder);
            Assert.Equal("SingleCommand", executionOrder[0]);
        }

        /// <summary>
        /// Helper command for testing that tracks execution order
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
        /// Helper command that always fails for testing exception handling
        /// </summary>
        private class FailingCommand : ICommand
        {
            private readonly string _name;
            private readonly List<string> _executionOrder;

            public FailingCommand(string name, List<string> executionOrder)
            {
                _name = name;
                _executionOrder = executionOrder;
            }

            public void Execute()
            {
                throw new InvalidOperationException($"{_name} failed");
            }
        }
    }
}