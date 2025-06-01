using System;
using System.Collections.Generic;
using SpaceGame.Core;
using SpaceGame.Commands;
using SpaceGame.Core.IoC;

using SpaceGame.IoC;

namespace SpaceGame.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🚀 SpaceGame Demo - Task 1 Implementation");
            Console.WriteLine("=========================================\n");

            try
            {
                // Initialize all dependencies
                Console.WriteLine("🔧 Initializing IoC dependencies...");
                new RegisterAllTask1Dependencies().Execute();
                Console.WriteLine("✅ All dependencies registered successfully!\n");

                // Demo 1: Basic Vector and Angle operations
                DemoBasicMath();

                // Demo 2: Move and Rotate commands
                DemoMovementCommands();

                // Demo 3: Macro commands
                DemoMacroCommands();

                // Demo 4: Send command
                DemoSendCommand();

                // Demo 5: CommandInjectable
                DemoCommandInjectable();

                // Demo 6: Long-running actions
                DemoLongRunningActions();

                Console.WriteLine("\n🎉 All demos completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void DemoBasicMath()
        {
            Console.WriteLine("📐 Demo 1: Vector and Angle Math");
            Console.WriteLine("--------------------------------");

            // Vector operations
            var v1 = new Vector(1, -1, 2);
            var v2 = new Vector(-1, 1, -2);
            var result = v1 + v2;
            Console.WriteLine($"Vector {v1} + {v2} = {result}");

            // Angle operations
            var a1 = new Angle(45, 360);
            var a2 = new Angle(30, 360);
            var angleResult = a1 + a2;
            Console.WriteLine($"Angle {a1} + {a2} = {angleResult}");
            Console.WriteLine($"cos({a1}) = {Math.Cos(a1):F3}, sin({a1}) = {Math.Sin(a1):F3}\n");
        }

        static void DemoMovementCommands()
        {
            Console.WriteLine("🚀 Demo 2: Movement Commands via IoC");
            Console.WriteLine("------------------------------------");

            // Create a spaceship
            var spaceship = new Dictionary<string, object>
            {
                ["Position"] = new Vector(10, 20),
                ["Velocity"] = new Vector(5, -3),
                ["Direction"] = new Angle(45, 360),
                ["AngularVelocity"] = new Angle(15, 360)
            };

            Console.WriteLine($"Initial position: {spaceship["Position"]}");
            Console.WriteLine($"Initial direction: {spaceship["Direction"]}");

            // Get commands via IoC
            var movingObject = Core.IoC.IoC.Resolve<IMovingObject>("Adapters.IMovingObject", spaceship);
            var rotatingObject = Core.IoC.IoC.Resolve<IRotatingObject>("Adapters.IRotatingObject", spaceship);

            var moveCommand = Core.IoC.IoC.Resolve<ICommand>("Commands.Move", movingObject);
            var rotateCommand = Core.IoC.IoC.Resolve<ICommand>("Commands.Rotate", rotatingObject);

            // Execute commands
            moveCommand.Execute();
            rotateCommand.Execute();

            Console.WriteLine($"After move: {spaceship["Position"]}");
            Console.WriteLine($"After rotate: {spaceship["Direction"]}\n");
        }

        static void DemoMacroCommands()
        {
            Console.WriteLine("🔄 Demo 3: Macro Commands");
            Console.WriteLine("------------------------");

            var executionLog = new List<string>();
            var cmd1 = new LoggingCommand("Command1", executionLog);
            var cmd2 = new LoggingCommand("Command2", executionLog);
            var cmd3 = new LoggingCommand("Command3", executionLog);

            var macroCommand = Core.IoC.IoC.Resolve<ICommand>("Commands.Macro",
                new ICommand[] { cmd1, cmd2, cmd3 });

            Console.WriteLine("Executing macro command with 3 sub-commands...");
            macroCommand.Execute();

            Console.WriteLine("Execution order:");
            foreach (var log in executionLog)
            {
                Console.WriteLine($"  ✓ {log}");
            }
            Console.WriteLine();
        }

        static void DemoSendCommand()
        {
            Console.WriteLine("📤 Demo 4: Send Command");
            Console.WriteLine("----------------------");

            var receiver = new ConsoleCommandReceiver();
            var testCommand = new LoggingCommand("TestCommand", new List<string>());

            var sendCommand = Core.IoC.IoC.Resolve<ICommand>("Commands.Send", testCommand, receiver);
            sendCommand.Execute();
            Console.WriteLine();
        }

        static void DemoCommandInjectable()
        {
            Console.WriteLine("💉 Demo 5: Command Injectable");
            Console.WriteLine("----------------------------");

            var injectable = Core.IoC.IoC.Resolve<ICommandInjectable>("Commands.CommandInjectable");
            var executable = Core.IoC.IoC.Resolve<ICommand>("Commands.CommandInjectable");

            var testCommand = new LoggingCommand("InjectedCommand", new List<string>());

            Console.WriteLine("Injecting command...");
            injectable.Inject(testCommand);

            Console.WriteLine("Executing injected command...");
            executable.Execute();
            Console.WriteLine();
        }

        static void DemoLongRunningActions()
        {
            Console.WriteLine("⏱️ Demo 6: Long-Running Actions");
            Console.WriteLine("------------------------------");

            var spaceship = new Dictionary<string, object>
            {
                ["Position"] = new Vector(0, 0),
                ["Velocity"] = new Vector(1, 1)
            };

            var commandQueue = new Queue<ICommand>();

            var startOrder = new Dictionary<string, object>
            {
                ["Object"] = spaceship,
                ["Operation"] = "Move",
                ["Queue"] = commandQueue
            };

            var stopOrder = new Dictionary<string, object>
            {
                ["Object"] = spaceship,
                ["Operation"] = "Move",
                ["Queue"] = commandQueue
            };

            // Start long-running operation
            var startAction = Core.IoC.IoC.Resolve<ICommand>("Actions.Start", startOrder);
            Console.WriteLine("Starting long-running Move operation...");
            startAction.Execute();

            Console.WriteLine($"Commands in queue: {commandQueue.Count}");
            var commandId = $"{spaceship.GetHashCode()}_Move";
            Console.WriteLine($"Command '{commandId}' registered: {CommandRegistry.Instance.CommandExists(commandId)}");

            // Stop the operation
            var stopAction = Core.IoC.IoC.Resolve<ICommand>("Actions.Stop", stopOrder);
            Console.WriteLine("Stopping long-running operation...");
            stopAction.Execute();

            Console.WriteLine($"Command '{commandId}' still registered: {CommandRegistry.Instance.CommandExists(commandId)}");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Helper command that logs its execution
    /// </summary>
    public class LoggingCommand : ICommand
    {
        private readonly string _name;
        private readonly List<string> _log;

        public LoggingCommand(string name, List<string> log)
        {
            _name = name;
            _log = log;
        }

        public void Execute()
        {
            _log.Add(_name);
            Console.WriteLine($"  Executed: {_name}");
        }
    }

    /// <summary>
    /// Simple command receiver for demo
    /// </summary>
    public class ConsoleCommandReceiver : ICommandReceiver
    {
        public void Receive(ICommand command)
        {
            Console.WriteLine($"Received command: {command.GetType().Name}");
            Console.WriteLine("Executing received command...");
            command.Execute();
        }
    }
}