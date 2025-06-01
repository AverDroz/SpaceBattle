using System;
using SpaceGame.Core;

namespace SpaceGame.Commands
{
    /// <summary>
    /// Command for moving objects (Task 1, Point 5)
    /// </summary>
    public class MoveCommand : ICommand
    {
        private readonly IMovingObject _movingObject;

        public MoveCommand(IMovingObject movingObject)
        {
            _movingObject = movingObject ?? throw new ArgumentNullException(nameof(movingObject));
        }

        public void Execute()
        {
            if (_movingObject.Position == null)
                throw new InvalidOperationException("Cannot determine object position");

            if (_movingObject.Velocity == null)
                throw new InvalidOperationException("Cannot determine object velocity");

            try
            {
                _movingObject.Position = _movingObject.Position + _movingObject.Velocity;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Cannot change object position", ex);
            }
        }
    }
}