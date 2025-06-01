using System;
using SpaceGame.Commands;
using SpaceGame.Core;
using SpaceGame.Core.IoC;

namespace SpaceGame.IoC
{
    /// <summary>
    /// Register CommandInjectableCommand dependency in IoC container (Task 18)
    /// </summary>
    public class RegisterIoCDependencyCommandInjectableCommand : ICommand
    {
        public void Execute()
        {
            // Create singleton instance that will be returned for all interface types
            var singleInstance = new CommandInjectableCommand();
            
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Commands.CommandInjectable",
                (object[] args) => singleInstance
            ).Execute();
        }
    }
}