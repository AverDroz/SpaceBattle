using Xunit;
using Moq;
using System;
using SpaceGame.Commands;
using SpaceGame.Core;
using SpaceGame.IoC;

namespace SpaceGame.Tests.Commands
{
    public class CommandInjectableCommandTests
    {
        [Fact]
        public void CommandInjectableCommand_Should_Execute_Injected_Command()
        {
            // Arrange
            var commandMock = new Mock<ICommand>();
            var injectedCommand = commandMock.Object;
            
            var commandInjectable = new CommandInjectableCommand();

            // Act
            commandInjectable.Inject(injectedCommand);
            commandInjectable.Execute();

            // Assert
            commandMock.Verify(c => c.Execute(), Times.Once);
        }

        [Fact]
        public void CommandInjectableCommand_Should_Throw_Exception_When_No_Command_Injected()
        {
            // Arrange
            var commandInjectable = new CommandInjectableCommand();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => commandInjectable.Execute());
            Assert.Equal("No command has been injected", exception.Message);
        }

        [Fact]
        public void CommandInjectableCommand_Should_Throw_ArgumentNullException_When_Injecting_Null()
        {
            // Arrange
            var commandInjectable = new CommandInjectableCommand();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => commandInjectable.Inject(null));
        }

        [Fact]
        public void CommandInjectableCommand_Should_Update_Injected_Command()
        {
            // Arrange
            var firstCommandMock = new Mock<ICommand>();
            var secondCommandMock = new Mock<ICommand>();
            
            var commandInjectable = new CommandInjectableCommand();

            // Act
            commandInjectable.Inject(firstCommandMock.Object);
            commandInjectable.Inject(secondCommandMock.Object); // Replace first command
            commandInjectable.Execute();

            // Assert
            firstCommandMock.Verify(c => c.Execute(), Times.Never);
            secondCommandMock.Verify(c => c.Execute(), Times.Once);
        }
    }

    public class CommandInjectableRegistrationTests
    {
        [Fact]
        public void RegisterIoCDependencyCommandInjectableCommand_Should_Allow_Multiple_Interface_Resolution()
        {
            // Arrange
            var registerCommand = new RegisterIoCDependencyCommandInjectableCommand();

            // Act
            registerCommand.Execute();

            // Assert - Should be able to resolve as different interface types
            var asICommand = Core.IoC.IoC.Resolve<ICommand>("Commands.CommandInjectable");
            var asICommandInjectable = Core.IoC.IoC.Resolve<ICommandInjectable>("Commands.CommandInjectable");
            var asConcreteType = Core.IoC.IoC.Resolve<CommandInjectableCommand>("Commands.CommandInjectable");

            Assert.NotNull(asICommand);
            Assert.NotNull(asICommandInjectable);
            Assert.NotNull(asConcreteType);

            // Verify they work as expected
            var testCommandMock = new Mock<ICommand>();
            asICommandInjectable.Inject(testCommandMock.Object);
            asICommand.Execute();

            testCommandMock.Verify(c => c.Execute(), Times.Once);
        }

        [Fact]
        public void Commands_CommandInjectable_Should_Work_End_To_End()
        {
            // Arrange
            var registerCommand = new RegisterIoCDependencyCommandInjectableCommand();
            registerCommand.Execute();

            var testCommandMock = new Mock<ICommand>();

            // Act
            var injectable = Core.IoC.IoC.Resolve<ICommandInjectable>("Commands.CommandInjectable");
            var executable = Core.IoC.IoC.Resolve<ICommand>("Commands.CommandInjectable");

            injectable.Inject(testCommandMock.Object);
            executable.Execute();

            // Assert
            testCommandMock.Verify(c => c.Execute(), Times.Once);
        }
    }
}