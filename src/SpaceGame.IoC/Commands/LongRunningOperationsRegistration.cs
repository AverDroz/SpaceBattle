using System;
using System.Collections.Generic;
using SpaceGame.Commands;

namespace SpaceGame.IoC
{
    /// <summary>
    /// Register Actions.Start dependency in IoC container (Task 19)
    /// </summary>
    public class RegisterIoCDependencyActionsStart : ICommand
    {
        public void Execute()
        {
            // Register shared registry instance
            var registry = new LongRunningOperationRegistry();
            Ioc.Resolve<ICommand>(
                "IoC.Register",
                "LongRunningOperationRegistry",
                (object[] args) => registry
            ).Execute();

            // Register Actions.Start dependency
            Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Actions.Start",
                (object[] args) =>
                {
                    if (args.Length != 1)
                        throw new ArgumentException("Actions.Start requires exactly one argument: order dictionary");

                    if (args[0] is not IDictionary<string, object> order)
                        throw new ArgumentException("First argument must be IDictionary<string, object> order");

                    // Extract required parameters from order
                    if (!order.TryGetValue("OperationId", out var operationIdObj) || operationIdObj is not string operationId)
                        throw new ArgumentException("Order must contain 'OperationId' as string");

                    if (!order.TryGetValue("OperationType", out var operationTypeObj) || operationTypeObj is not string operationType)
                        throw new ArgumentException("Order must contain 'OperationType' as string");

                    if (!order.TryGetValue("GameObject", out var gameObject))
                        throw new ArgumentException("Order must contain 'GameObject'");

                    // Get registry instance
                    var operationRegistry = Ioc.Resolve<LongRunningOperationRegistry>("LongRunningOperationRegistry");

                    return new StartLongRunningOperationCommand(operationId, operationType, gameObject, operationRegistry);
                }
            ).Execute();

            // Register helper dependencies for command injection
            RegisterCommandInjectableDependency();
            RegisterSendCommandDependency();
        }

        private void RegisterCommandInjectableDependency()
        {
            Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Commands.CommandInjectable.Start",
                (object[] args) =>
                {
                    if (args.Length != 1)
                        throw new ArgumentException("Commands.CommandInjectable.Start requires one argument: order");

                    var order = args[0] as IDictionary<string, object>;
                    var startCommand = Ioc.Resolve<ICommand>("Actions.Start", order);
                    var injectableCommand = Ioc.Resolve<CommandInjectableCommand>("Commands.CommandInjectable");
                    
                    injectableCommand.Inject(startCommand);
                    return injectableCommand;
                }
            ).Execute();
        }

        private void RegisterSendCommandDependency()
        {
            Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Send.Start",
                (object[] args) =>
                {
                    if (args.Length != 2)
                        throw new ArgumentException("Commands.Send.Start requires two arguments: order and receiver");

                    var order = args[0] as IDictionary<string, object>;
                    var receiver = args[1] as ICommandReceiver;
                    
                    var startCommand = Ioc.Resolve<ICommand>("Actions.Start", order);
                    return new SendCommand(startCommand, receiver);
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
            // Register Actions.Stop dependency
            Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Actions.Stop",
                (object[] args) =>
                {
                    if (args.Length != 1)
                        throw new ArgumentException("Actions.Stop requires exactly one argument: order dictionary");

                    if (args[0] is not IDictionary<string, object> order)
                        throw new ArgumentException("First argument must be IDictionary<string, object> order");

                    // Extract operation ID from order
                    if (!order.TryGetValue("OperationId", out var operationIdObj) || operationIdObj is not string operationId)
                        throw new ArgumentException("Order must contain 'OperationId' as string");

                    // Get registry instance
                    var registry = Ioc.Resolve<LongRunningOperationRegistry>("LongRunningOperationRegistry");

                    return new StopLongRunningOperationCommand(operationId, registry);
                }
            ).Execute();

            // Register helper dependencies
            RegisterCommandInjectableStopDependency();
            RegisterSendCommandStopDependency();
        }

        private void RegisterCommandInjectableStopDependency()
        {
            Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Commands.CommandInjectable.Stop",
                (object[] args) =>
                {
                    if (args.Length != 1)
                        throw new ArgumentException("Commands.CommandInjectable.Stop requires one argument: order");

                    var order = args[0] as IDictionary<string, object>;
                    var stopCommand = Ioc.Resolve<ICommand>("Actions.Stop", order);
                    var injectableCommand = Ioc.Resolve<CommandInjectableCommand>("Commands.CommandInjectable");
                    
                    injectableCommand.Inject(stopCommand);
                    return injectableCommand;
                }
            ).Execute();
        }

        private void RegisterSendCommandStopDependency()
        {
            Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Send.Stop",
                (object[] args) =>
                {
                    if (args.Length != 2)
                        throw new ArgumentException("Commands.Send.Stop requires two arguments: order and receiver");

                    var order = args[0] as IDictionary<string, object>;
                    var receiver = args[1] as ICommandReceiver;
                    
                    var stopCommand = Ioc.Resolve<ICommand>("Actions.Stop", order);
                    return new SendCommand(stopCommand, receiver);
                }
            ).Execute();
        }
    }
}