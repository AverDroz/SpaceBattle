using System;
using SpaceGame.Core;

namespace SpaceGame.Commands
{
    /// <summary>
    /// Command that executes a sequence of commands without using loops
    /// Implements recursive approach as required by task specification
    /// </summary>
    public class MacroCommand : ICommand
    {
        private readonly ICommand[] _commands;
        private readonly int _currentIndex;

        /// <summary>
        /// Creates MacroCommand with array of commands to execute
        /// </summary>
        /// <param name="commands">Array of commands to execute sequentially</param>
        public MacroCommand(ICommand[] commands) : this(commands, 0)
        {
        }

        /// <summary>
        /// Private constructor for recursive execution
        /// </summary>
        /// <param name="commands">Array of commands</param>
        /// <param name="currentIndex">Current command index</param>
        private MacroCommand(ICommand[] commands, int currentIndex)
        {
            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
            _currentIndex = currentIndex;
        }

        /// <summary>
        /// Executes commands sequentially without using loops (recursive approach)
        /// </summary>
        public void Execute()
        {
            // Base case: if we've executed all commands, return
            if (_currentIndex >= _commands.Length)
                return;

            // Execute current command
            var currentCommand = _commands[_currentIndex];
            if (currentCommand == null)
                throw new InvalidOperationException($"Command at index {_currentIndex} is null");

            try
            {
                currentCommand.Execute();
            }
            catch (Exception)
            {
                throw;
            }

            // Recursively execute remaining commands only if current command succeeded
            var nextMacroCommand = new MacroCommand(_commands, _currentIndex + 1);
            nextMacroCommand.Execute();
        }
    }
}