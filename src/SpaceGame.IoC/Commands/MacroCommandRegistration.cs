using System;
using SpaceGame.Commands;

namespace SpaceGame.IoC
{
    /// <summary>
    /// Register MacroCommand dependency in IoC container (Task 11)
    /// </summary>
    public class RegisterIoCDependencyMacroCommand : ICommand
    {
        public void Execute()
        {
            Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Macro",
                (object[] args) =>
                {
                    if (args.Length != 1)
                        throw new ArgumentException("Commands.Macro requires exactly one argument: array of commands");

                    if (args[0] is not ICommand[] commands)
                        throw new ArgumentException("First argument must be ICommand[] array");

                    return new MacroCommand(commands);
                }
            ).Execute();
        }
    }

    /// <summary>
    /// Strategy for creating MacroCommand from command specifications (Task 12)
    /// </summary>
    public class CreateMacroCommandStrategy
    {
        private readonly string _commandSpec;

        public CreateMacroCommandStrategy(string commandSpec)
        {
            _commandSpec = commandSpec ?? throw new ArgumentNullException(nameof(commandSpec));
        }

        /// <summary>
        /// Resolves MacroCommand based on command specification without using loops
        /// </summary>
        /// <param name="args">Arguments for command resolution</param>
        /// <returns>MacroCommand instance</returns>
        public ICommand Resolve(object[] args)
        {
            // Get list of command names from specification
            var commandNames = Ioc.Resolve<string[]>(_commandSpec);
            
            if (commandNames == null)
                throw new InvalidOperationException($"Command specification '{_commandSpec}' not found");

            // Create commands array from command names recursively
            var commands = CreateCommandsArray(commandNames, 0, args);
            
            // Create and return MacroCommand
            return Ioc.Resolve<ICommand>("Commands.Macro", commands);
        }

        /// <summary>
        /// Recursively creates array of commands from command names (no loops used)
        /// </summary>
        /// <param name="commandNames">Array of command names</param>
        /// <param name="currentIndex">Current index in array</param>
        /// <param name="args">Arguments for command resolution</param>
        /// <returns>Array of resolved commands</returns>
        private ICommand[] CreateCommandsArray(string[] commandNames, int currentIndex, object[] args)
        {
            // Base case: if we've processed all command names, return empty array
            if (currentIndex >= commandNames.Length)
                return new ICommand[0];

            // Resolve current command
            var currentCommand = Ioc.Resolve<ICommand>(commandNames[currentIndex], args);
            
            // Recursively get remaining commands
            var remainingCommands = CreateCommandsArray(commandNames, currentIndex + 1, args);
            
            // Create new array with current command + remaining commands
            var result = new ICommand[remainingCommands.Length + 1];
            result[0] = currentCommand;
            CopyArray(remainingCommands, result, 1);
            
            return result;
        }

        /// <summary>
        /// Helper method to copy array without using loops (recursive approach)
        /// </summary>
        /// <param name="source">Source array</param>
        /// <param name="destination">Destination array</param>
        /// <param name="startIndex">Start index in destination</param>
        private void CopyArray(ICommand[] source, ICommand[] destination, int startIndex)
        {
            CopyArrayRecursive(source, destination, 0, startIndex);
        }

        /// <summary>
        /// Recursive array copy implementation
        /// </summary>
        /// <param name="source">Source array</param>
        /// <param name="destination">Destination array</param>
        /// <param name="sourceIndex">Current source index</param>
        /// <param name="destIndex">Current destination index</param>
        private void CopyArrayRecursive(ICommand[] source, ICommand[] destination, int sourceIndex, int destIndex)
        {
            // Base case: if we've copied all elements, return
            if (sourceIndex >= source.Length)
                return;

            // Copy current element
            destination[destIndex] = source[sourceIndex];
            
            // Recursively copy remaining elements
            CopyArrayRecursive(source, destination, sourceIndex + 1, destIndex + 1);
        }
    }

    /// <summary>
    /// Register Macro.Move and Macro.Rotate dependencies (Task 13)
    /// </summary>
    public class RegisterIoCDependencyMacroMoveRotate : ICommand
    {
        public void Execute()
        {
            // Register Macro.Move dependency
            Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Macro.Move",
                (object[] args) =>
                {
                    var strategy = new CreateMacroCommandStrategy("Specs.Move");
                    return strategy.Resolve(args);
                }
            ).Execute();

            // Register Macro.Rotate dependency
            Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Macro.Rotate",
                (object[] args) =>
                {
                    var strategy = new CreateMacroCommandStrategy("Specs.Rotate");
                    return strategy.Resolve(args);
                }
            ).Execute();
        }
    }
}