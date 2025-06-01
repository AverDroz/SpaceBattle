using System;
using SpaceGame.Core;

namespace SpaceGame.Commands
{
    /// <summary>
    /// Interface for receiving commands (Task 14)
    /// </summary>
    public interface ICommandReceiver
    {
        /// <summary>
        /// Receives a command for processing
        /// </summary>
        /// <param name="command">Command to receive</param>
        void Receive(ICommand command);
    }

    /// <summary>
    /// Command for sending commands to a receiver (Task 14)
    /// </summary>
    public class SendCommand : ICommand
    {
        private readonly ICommand _command;
        private readonly ICommandReceiver _receiver;

        /// <summary>
        /// Creates SendCommand with command and receiver
        /// </summary>
        /// <param name="command">Command to send</param>
        /// <param name="receiver">Receiver to send command to</param>
        public SendCommand(ICommand command, ICommandReceiver receiver)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }

        /// <summary>
        /// Executes SendCommand by calling Receive on the receiver
        /// </summary>
        public void Execute()
        {
            try
            {
                _receiver.Receive(_command);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ICommandReceiver cannot accept the command", ex);
            }
        }
    }
}