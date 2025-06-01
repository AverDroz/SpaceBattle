using System;

namespace SpaceGame.Commands
{
    /// <summary>
    /// Interface for command injection (Task 16)
    /// </summary>
    public interface ICommandInjectable
    {
        /// <summary>
        /// Injects a command into the object
        /// </summary>
        /// <param name="command">Command to inject</param>
        void Inject(ICommand command);
    }

    /// <summary>
    /// Command that implements both ICommand and ICommandInjectable (Task 17)
    /// </summary>
    public class CommandInjectableCommand : ICommand, ICommandInjectable
    {
        private ICommand _injectedCommand;

        /// <summary>
        /// Injects a command to be executed when Execute is called
        /// </summary>
        /// <param name="command">Command to inject</param>
        public void Inject(ICommand command)
        {
            _injectedCommand = command ?? throw new ArgumentNullException(nameof(command));
        }

        /// <summary>
        /// Executes the injected command
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when no command has been injected</exception>
        public void Execute()
        {
            if (_injectedCommand == null)
                throw new InvalidOperationException("No command has been injected");

            _injectedCommand.Execute();
        }
    }
}