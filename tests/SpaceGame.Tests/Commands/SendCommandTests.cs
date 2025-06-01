using Xunit;
using Moq;
using System;
using SpaceGame.Commands;
using SpaceGame.IoC;
using SpaceGame.Core;

namespace SpaceGame.Tests.Commands
{
    public class SendCommandTests
    {
        [Fact]
        public void SendCommand_Should_Send_Command_To_Receiver()
        {
            // Arrange
            var commandMock = new Mock<ICommand>();
            var command = commandMock.Object;

            var receiverMock = new Mock<ICommandReceiver>();
            var receiver = receiverMock.Object;

            var sendCommand = new SendCommand(command, receiver);

            // Act
            sendCommand.Execute();

            // Assert
            receiverMock.Verify(r => r.Receive(command), Times.Once);
        }

        [Fact]
        public void SendCommand_Should_Throw_Exception_When_Receiver_Cannot_Accept_Command()
        {
            // Arrange
            var commandMock = new Mock<ICommand>();
            var command = commandMock.Object;

            var receiverMock = new Mock<ICommandReceiver>();
            receiverMock.Setup(r => r.Receive(It.IsAny<ICommand>()))
                       .Throws(new InvalidOperationException("Cannot accept command"));

            var sendCommand = new SendCommand(command, receiverMock.Object);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => sendCommand.Execute());
            Assert.Equal("ICommandReceiver cannot accept the command", exception.Message);
        }

        [Fact]
        public void SendCommand_Should_Throw_ArgumentNullException_When_Command_Is_Null()
        {
            // Arrange
            var receiverMock = new Mock<ICommandReceiver>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SendCommand(null, receiverMock.Object));
        }

        [Fact]
        public void SendCommand_Should_Throw_ArgumentNullException_When_Receiver_Is_Null()
        {
            // Arrange
            var commandMock = new Mock<ICommand>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SendCommand(commandMock.Object, null));
        }
    }

    public class SendCommandRegistrationTests
    {
        [Fact]
        public void RegisterIoCDependencySendCommand_Should_Register_Commands_Send_Dependency()
        {
            // Arrange
            var registerCommand = new RegisterIoCDependencySendCommand();

            // Act
            registerCommand.Execute();

            // Assert
            var commandMock = new Mock<ICommand>();
            var receiverMock = new Mock<ICommandReceiver>();

            var sendCommand = Core.IoC.IoC.Resolve<ICommand>("Commands.Send", 
                commandMock.Object, receiverMock.Object);

            Assert.NotNull(sendCommand);
            Assert.IsType<SendCommand>(sendCommand);

            // Verify it works
            sendCommand.Execute();
            receiverMock.Verify(r => r.Receive(commandMock.Object), Times.Once);
        }

        [Fact]
        public void Commands_Send_Should_Throw_Exception_With_Wrong_Number_Of_Arguments()
        {
            // Arrange
            var registerCommand = new RegisterIoCDependencySendCommand();
            registerCommand.Execute();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Core.IoC.IoC.Resolve<ICommand>("Commands.Send", new Mock<ICommand>().Object));
        }

        [Fact]
        public void Commands_Send_Should_Throw_Exception_With_Wrong_Argument_Types()
        {
            // Arrange
            var registerCommand = new RegisterIoCDependencySendCommand();
            registerCommand.Execute();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Core.IoC.IoC.Resolve<ICommand>("Commands.Send", "not a command", "not a receiver"));
        }
    }
}