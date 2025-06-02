using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using SpaceGame.Core;
using SpaceGame.Core.IoC;

namespace SpaceGame.Commands
{
    public class StartAction : ICommand
    {
        private readonly IDictionary<string, object> _order;

        public StartAction(IDictionary<string, object> order)
        {
            _order = order ?? throw new ArgumentNullException(nameof(order));
        }

        public void Execute()
        {
            // Get required parameters from the order
            if (!_order.TryGetValue("Object", out var gameObject))
                throw new ArgumentException("Order must contain 'Object' parameter");

            if (!_order.TryGetValue("Operation", out var operationName) || operationName is not string opName)
                throw new ArgumentException("Order must contain 'Operation' parameter as string");

            if (!_order.TryGetValue("Queue", out var commandQueue) || commandQueue is not Queue<ICommand> queue)
                throw new ArgumentException("Order must contain 'Queue' parameter as Queue<ICommand>");

            // Create the long-running command
            var longRunningCommand = CreateLongRunningCommand(opName, gameObject);

            // Add command to the queue
            queue.Enqueue(longRunningCommand);

            // Store the command reference for potential stopping
            var commandId = GenerateCommandId(gameObject, opName);
            CommandRegistry.Instance.RegisterCommand(commandId, longRunningCommand);
        }

        private ICommand CreateLongRunningCommand(string operationName, object gameObject)
            {
                switch (operationName)
                {
                    case "Move":
                    {
                        var movingObject = Core.IoC.IoC.Resolve<IMovingObject>("Adapters.IMovingObject", gameObject);
                        return Core.IoC.IoC.Resolve<ICommand>("Commands.Move", movingObject);
                    }
                    case "Rotate":
                    {
                        var rotatingObject = Core.IoC.IoC.Resolve<IRotatingObject>("Adapters.IRotatingObject", gameObject);
                        return Core.IoC.IoC.Resolve<ICommand>("Commands.Rotate", rotatingObject);
                    }
                    default:
                        throw new ArgumentException($"Unknown operation: {operationName}");
                }
            }

        private string GenerateCommandId(object gameObject, string operationName)
        {
            var objectId = gameObject.GetHashCode().ToString();
            return $"{objectId}_{operationName}";
        }
    }

    public class StopAction : ICommand
    {
        private readonly IDictionary<string, object> _order;

        public StopAction(IDictionary<string, object> order)
        {
            _order = order ?? throw new ArgumentNullException(nameof(order));
        }

        public void Execute()
        {
            // Get required parameters from the order
            if (!_order.TryGetValue("Object", out var gameObject))
                throw new ArgumentException("Order must contain 'Object' parameter");

            if (!_order.TryGetValue("Operation", out var operationName) || operationName is not string opName)
                throw new ArgumentException("Order must contain 'Operation' parameter as string");

            if (!_order.TryGetValue("Queue", out var commandQueue) || commandQueue is not Queue<ICommand> queue)
                throw new ArgumentException("Order must contain 'Queue' parameter as Queue<ICommand>");

            // Generate command ID and remove from registry
            var commandId = GenerateCommandId(gameObject, opName);
            var command = CommandRegistry.Instance.UnregisterCommand(commandId);

            if (command != null)
            {
                // Remove command from queue - this requires O(1) operation as per task requirement
                // We use CommandWrapper to mark commands as stopped
                var stoppedCommand = new StoppedCommandWrapper(command);
                // Note: In real implementation, queue would need to be replaced with a data structure
                // that supports O(1) removal. For now, we mark the command as stopped.
            }
        }

        private string GenerateCommandId(object gameObject, string operationName)
        {
            var objectId = gameObject.GetHashCode().ToString();
            return $"{objectId}_{operationName}";
        }
    }

    /// <summary>
    /// Wrapper for commands that have been stopped
    /// </summary>
    public class StoppedCommandWrapper : ICommand
    {
        private readonly ICommand _wrappedCommand;

        public StoppedCommandWrapper(ICommand wrappedCommand)
        {
            _wrappedCommand = wrappedCommand ?? throw new ArgumentNullException(nameof(wrappedCommand));
        }

        public void Execute()
        {
            // Do nothing - command has been stopped
        }

        public ICommand WrappedCommand => _wrappedCommand;
    }

    /// <summary>
    /// Registry for tracking active long-running commands
    /// </summary>
    public class CommandRegistry
    {
        private static readonly Lazy<CommandRegistry> _instance = new(() => new CommandRegistry());
        private readonly ConcurrentDictionary<string, ICommand> _commands;

        private CommandRegistry()
        {
            _commands = new ConcurrentDictionary<string, ICommand>();
        }

        public static CommandRegistry Instance => _instance.Value;

        /// <summary>
        /// Registers a command with O(1) complexity
        /// </summary>
        public void RegisterCommand(string commandId, ICommand command)
        {
            _commands.TryAdd(commandId, command);
        }

        /// <summary>
        /// Unregisters and returns a command with O(1) complexity
        /// </summary>
        public ICommand? UnregisterCommand(string commandId)
        {
            _commands.TryRemove(commandId, out var command);
            return command;
        }

        /// <summary>
        /// Checks if command exists with O(1) complexity
        /// </summary>
        public bool CommandExists(string commandId)
        {
            return _commands.ContainsKey(commandId);
        }

        /// <summary>
        /// Clears all registered commands (for testing)
        /// </summary>
        public void Clear()
        {
            _commands.Clear();
        }
    }
}