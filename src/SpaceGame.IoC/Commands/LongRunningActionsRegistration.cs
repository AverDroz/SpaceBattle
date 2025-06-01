using System;
using System.Collections.Generic;
using SpaceGame.Commands;
using SpaceGame.Core;
using SpaceGame.Core.IoC;

namespace SpaceGame.IoC
{
    /// <summary>
    /// Register Actions.Start dependency in IoC container (Task 19)
    /// </summary>
    public class RegisterIoCDependencyActionsStart : ICommand
    {
        public void Execute()
        {
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Actions.Start",
                (object[] args) =>
                {
                    if (args.Length != 1)
                        throw new ArgumentException("Actions.Start requires exactly one argument: order dictionary");

                    if (args[0] is not IDictionary<string, object> order)
                        throw new ArgumentException("First argument must be IDictionary<string, object>");

                    return new StartAction(order);
                }
            ).Execute();
        }
    }

    /// <summary>
    /// Register Actions.Stop dependency in IoC container (Task 20)
    /// </summary>
    public class RegisterIoCDependencyActionsStop : ICommand
    {
        public void Execute()
        {
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Actions.Stop",
                (object[] args) =>
                {
                    if (args.Length != 1)
                        throw new ArgumentException("Actions.Stop requires exactly one argument: order dictionary");

                    if (args[0] is not IDictionary<string, object> order)
                        throw new ArgumentException("First argument must be IDictionary<string, object>");

                    return new StopAction(order);
                }
            ).Execute();
        }
    }

    /// <summary>
    /// Register all dependencies needed for long-running actions
    /// </summary>
    public class RegisterLongRunningActionsDependencies : ICommand
    {
        public void Execute()
        {
            new RegisterIoCDependencyActionsStart().Execute();
            new RegisterIoCDependencyActionsStop().Execute();

            // Register CommandRegistry as singleton
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "CommandRegistry",
                (object[] args) => CommandRegistry.Instance
            ).Execute();
        }
    }
}