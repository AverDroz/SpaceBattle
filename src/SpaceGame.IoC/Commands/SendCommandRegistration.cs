using System;
using SpaceGame.Commands;

namespace SpaceGame.IoC
{
    /// <summary>
    /// Register SendCommand dependency in IoC container (Task 15)
    /// </summary>
    public class RegisterIoCDependencySendCommand : ICommand
    {
        public void Execute()
        {
            Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Send",
                (object[] args) =>
                {
                    if (args.Length != 2)
                        throw new ArgumentException("Commands.Send requires exactly two arguments: command and receiver");

                    if (args[0] is not ICommand command)
                        throw new ArgumentException("First argument must be ICommand");

                    if (args[1] is not ICommandReceiver receiver)
                        throw new ArgumentException("Second argument must be ICommandReceiver");

                    return new SendCommand(command, receiver);
                }
            ).Execute();
        }
    }
}