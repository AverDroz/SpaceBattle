using System;
using System.Collections.Generic;
using SpaceGame.Core;
using SpaceGame.Core.IoC;
using SpaceGame.Commands;

namespace SpaceGame.IoC
{
    /// <summary>
    /// Register MoveCommand dependency in IoC container
    /// </summary>
    public class RegisterIoCDependencyMoveCommand : ICommand
    {
        public void Execute()
        {
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Move",
                (object[] args) =>
                {
                    if (args.Length != 1)
                        throw new ArgumentException("Commands.Move requires exactly one argument: moving object");

                    if (args[0] is IMovingObject movingObject)
                        return new MoveCommand(movingObject);

                    throw new ArgumentException("First argument must be IMovingObject");
                }
            ).Execute();

            // Also register adapter resolution
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Adapters.IMovingObject",
                (object[] args) =>
                {
                    if (args.Length != 1)
                        throw new ArgumentException("Adapters.IMovingObject requires exactly one argument: game object");

                    if (args[0] is IDictionary<string, object> gameObject)
                        return new MovingObjectAdapter(gameObject);

                    throw new ArgumentException("First argument must be IDictionary<string, object>");
                }
            ).Execute();
        }
    }

    /// <summary>
    /// Register RotateCommand dependency in IoC container 
    /// </summary>
    public class RegisterIoCDependencyRotateCommand : ICommand
    {
        public void Execute()
        {
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Rotate",
                (object[] args) =>
                {
                    if (args.Length != 1)
                        throw new ArgumentException("Commands.Rotate requires exactly one argument: rotating object");

                    if (args[0] is IRotatingObject rotatingObject)
                        return new RotateCommand(rotatingObject);

                    throw new ArgumentException("First argument must be IRotatingObject");
                }
            ).Execute();

            // Also register adapter resolution
            Core.IoC.IoC.Resolve<ICommand>(
                "IoC.Register",
                "Adapters.IRotatingObject",
                (object[] args) =>
                {
                    if (args.Length != 1)
                        throw new ArgumentException("Adapters.IRotatingObject requires exactly one argument: game object");

                    if (args[0] is IDictionary<string, object> gameObject)
                        return new RotatingObjectAdapter(gameObject);

                    throw new ArgumentException("First argument must be IDictionary<string, object>");
                }
            ).Execute();
        }
    }
}