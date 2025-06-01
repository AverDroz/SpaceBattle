using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using SpaceGame.Commands;
using SpaceGame.IoC;
using SpaceGame.Core;

namespace SpaceGame.Tests.IoC
{
    public class MacroCommandRegistrationTests
    {
        [Fact]
        public void RegisterIoCDependencyMacroCommand_Should_Register_Commands_Macro_Dependency()
        {
            // Arrange
            var registerCommand = new RegisterIoCDependencyMacroCommand();

            // Act
            registerCommand.Execute();

            // Assert
            var executionOrder = new List<string>();
            var command1 = new TestCommand("Test1", executionOrder);
            var command2 = new TestCommand("Test2", executionOrder);
            var commands = new ICommand[] { command1, command2 };

            var macroCommand = Core.IoC.IoC.Resolve<ICommand>("Commands.Macro", commands);
            
            Assert.NotNull(macroCommand);
            Assert.IsType<MacroCommand>(macroCommand);

            // Verify it works
            macroCommand.Execute();
            Assert.Equal(new[] { "Test1", "Test2" }, executionOrder);
        }

        [Fact]
        public void CreateMacroCommandStrategy_Should_Resolve_MacroCommand_Successfully()
        {
            // Arrange
            new RegisterIoCDependencyMacroCommand().Execute();
            
            // Mock command specifications
            var mockCommandNames = new string[] { "Commands.Test1", "Commands.Test2" };
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Specs.Test",
                (object[] args) => mockCommandNames
            ).Execute();

            // Mock individual commands
            var executionOrder = new List<string>();
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Test1",
                (object[] args) => new TestCommand("Test1", executionOrder)
            ).Execute();

            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Test2",
                (object[] args) => new TestCommand("Test2", executionOrder)
            ).Execute();

            var strategy = new CreateMacroCommandStrategy("Specs.Test");

            // Act
            var macroCommand = strategy.Resolve(new object[0]);

            // Assert
            Assert.NotNull(macroCommand);
            macroCommand.Execute();
            Assert.Equal(new[] { "Test1", "Test2" }, executionOrder);
        }

        [Fact]
        public void CreateMacroCommandStrategy_Should_Throw_Exception_When_Spec_Not_Found()
        {
            // Arrange
            var strategy = new CreateMacroCommandStrategy("Specs.NonExistent");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => strategy.Resolve(new object[0]));
        }

        [Fact]
        public void CreateMacroCommandStrategy_Should_Throw_Exception_When_Command_Not_Found()
        {
            // Arrange
            new RegisterIoCDependencyMacroCommand().Execute();
            
            // Mock command specifications with non-existent command
            var mockCommandNames = new string[] { "Commands.NonExistent" };
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Specs.TestFail",
                (object[] args) => mockCommandNames
            ).Execute();

            var strategy = new CreateMacroCommandStrategy("Specs.TestFail");

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => strategy.Resolve(new object[0]));
        }

        [Fact]
        public void RegisterIoCDependencyMacroMoveRotate_Should_Register_Macro_Dependencies()
        {
            // Arrange
            new RegisterIoCDependencyMacroCommand().Execute();
            
            // Mock Specs.Move and Specs.Rotate
            var moveSpecs = new string[] { "Commands.Move" };
            var rotateSpecs = new string[] { "Commands.Rotate" };
            
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Specs.Move",
                (object[] args) => moveSpecs
            ).Execute();

            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Specs.Rotate",
                (object[] args) => rotateSpecs
            ).Execute();

            // Mock the individual commands
            var executionOrder = new List<string>();
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Move",
                (object[] args) => new TestCommand("Move", executionOrder)
            ).Execute();

            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Rotate",
                (object[] args) => new TestCommand("Rotate", executionOrder)
            ).Execute();

            var registerCommand = new RegisterIoCDependencyMacroMoveRotate();

            // Act
            registerCommand.Execute();

            // Assert
            var macroMoveCommand = Core.IoC.IoC.Resolve<ICommand>("Macro.Move", new object[0]);
            var macroRotateCommand = Core.IoC.IoC.Resolve<ICommand>("Macro.Rotate", new object[0]);

            Assert.NotNull(macroMoveCommand);
            Assert.NotNull(macroRotateCommand);

            // Verify they execute correctly
            macroMoveCommand.Execute();
            macroRotateCommand.Execute();
            
            Assert.Contains("Move", executionOrder);
            Assert.Contains("Rotate", executionOrder);
        }

        /// <summary>
        /// Helper command for testing
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
    }
}