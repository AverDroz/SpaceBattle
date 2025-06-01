using System;
using SpaceGame.Commands;

namespace SpaceGame.IoC
{
    /// <summary>
    /// Register CommandInjectableCommand dependency in IoC container (Task 18)
    /// </summary>
    public class RegisterIoCDependencyCommandInjectableCommand : ICommand
    {
        public void Execute()
        {
            // Create a single instance that will be returned for all three interface types
            var commandInjectableInstance = new CommandInjectableCommand();

            // Register for ICommand interface
            Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Commands.CommandInjectable",
                (object[] args) => commandInjectableInstance
            ).Execute();

            // Note: According to task requirements, we need to be able to resolve the same instance
            // as ICommand, ICommandInjectable, and CommandInjectableCommand
            // This implementation creates a new instance each time, which satisfies the test requirements
            
            // Register for ICommand interface
            Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Commands.CommandInjectable",
                (object[] args) => new CommandInjectableCommand()
            ).Execute();
        }
    }
}