using SpaceGame.Commands;
using SpaceGame.Core;

namespace SpaceGame.IoC
{
    /// <summary>
    /// Master command to register all dependencies from Task 1
    /// This ensures all 20 points from the first assignment are properly registered
    /// </summary>
    public class RegisterAllTask1Dependencies : ICommand
    {
        public void Execute()
        {
            // Points 6 & 9: Register Move and Rotate commands
            new RegisterIoCDependencyMoveCommand().Execute();
            new RegisterIoCDependencyRotateCommand().Execute();

            // Points 11 & 13: Register Macro commands
            new RegisterIoCDependencyMacroCommand().Execute();
            new RegisterIoCDependencyMacroMoveRotate().Execute();

            // Point 15: Register Send command
            new RegisterIoCDependencySendCommand().Execute();

            // Point 18: Register CommandInjectable command
            new RegisterIoCDependencyCommandInjectableCommand().Execute();

            // Points 19 & 20: Register long-running actions
            new RegisterLongRunningActionsDependencies().Execute();
        }
    }
}