using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using SpaceGame.Core;
using SpaceGame.Core.IoC;

namespace SpaceGame.Commands
{
    /// <summary>
    /// Registry for tracking long-running operations
    /// </summary>
    public class LongRunningOperationRegistry
    {
        private readonly ConcurrentDictionary<string, ICommand> _activeOperations;

        public LongRunningOperationRegistry()
        {
            _activeOperations = new ConcurrentDictionary<string, ICommand>();
        }

        /// <summary>
        /// Starts a long-running operation by adding command to queue
        /// </summary>
        /// <param name="operationId">Unique operation identifier</param>
        /// <param name="command">Command to execute continuously</param>
        public void StartOperation(string operationId, ICommand command)
        {
            if (string.IsNullOrEmpty(operationId))
                throw new ArgumentException("Operation ID cannot be null or empty", nameof(operationId));
            
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (!_activeOperations.TryAdd(operationId, command))
                throw new InvalidOperationException($"Operation '{operationId}' is already running");

            // In real implementation, this would add command to a processing queue
            // For now, we just track it in the registry
        }

        /// <summary>
        /// Stops a long-running operation by removing it from queue
        /// </summary>
        /// <param name="operationId">Operation identifier to stop</param>
        /// <returns>True if operation was stopped, false if it wasn't running</returns>
        public bool StopOperation(string operationId)
        {
            if (string.IsNullOrEmpty(operationId))
                return false;

            return _activeOperations.TryRemove(operationId, out _);
        }

        /// <summary>
        /// Checks if operation is currently running
        /// </summary>
        /// <param name="operationId">Operation identifier</param>
        /// <returns>True if operation is active</returns>
        public bool IsOperationActive(string operationId)
        {
            return !string.IsNullOrEmpty(operationId) && _activeOperations.ContainsKey(operationId);
        }

        /// <summary>
        /// Gets all active operation IDs
        /// </summary>
        /// <returns>Collection of active operation IDs</returns>
        public IEnumerable<string> GetActiveOperationIds()
        {
            return _activeOperations.Keys;
        }
    }

    /// <summary>
    /// Command for starting long-running operations (Task 19)
    /// </summary>
    public class StartLongRunningOperationCommand : ICommand
    {
        private readonly string _operationId;
        private readonly string _operationType;
        private readonly object _gameObject;
        private readonly LongRunningOperationRegistry _registry;

        public StartLongRunningOperationCommand(
            string operationId, 
            string operationType, 
            object gameObject, 
            LongRunningOperationRegistry registry)
        {
            _operationId = operationId ?? throw new ArgumentNullException(nameof(operationId));
            _operationType = operationType ?? throw new ArgumentNullException(nameof(operationType));
            _gameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public void Execute()
        {
            // Resolve the actual command for the operation type
            var operationCommand = Core.IoC.IoC.Resolve<ICommand>($"Commands.{_operationType}", _gameObject);
            
            // Start the long-running operation
            _registry.StartOperation(_operationId, operationCommand);
        }
    }

    /// <summary>
    /// Command for stopping long-running operations (Task 20)
    /// </summary>
    public class StopLongRunningOperationCommand : ICommand
    {
        private readonly string _operationId;
        private readonly LongRunningOperationRegistry _registry;

        public StopLongRunningOperationCommand(string operationId, LongRunningOperationRegistry registry)
        {
            _operationId = operationId ?? throw new ArgumentNullException(nameof(operationId));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public void Execute()
        {
            // Stop operation in constant time as required
            var stopped = _registry.StopOperation(_operationId);
            
            if (!stopped)
                throw new InvalidOperationException($"Operation '{_operationId}' was not running");
        }
    }
}