using System;
using SpaceGame.Core;

namespace SpaceGame.Commands
{
    /// <summary>
    /// Command for rotating objects (Task 1, Point 8)
    /// </summary>
    public class RotateCommand : ICommand
    {
        private readonly IRotatingObject _rotatingObject;

        public RotateCommand(IRotatingObject rotatingObject)
        {
            _rotatingObject = rotatingObject ?? throw new ArgumentNullException(nameof(rotatingObject));
        }

        public void Execute()
        {
            if (_rotatingObject.Direction == null)
                throw new InvalidOperationException("Cannot determine object direction");

            if (_rotatingObject.AngularVelocity == null)
                throw new InvalidOperationException("Cannot determine object angular velocity");

            try
            {
                _rotatingObject.Direction = _rotatingObject.Direction + _rotatingObject.AngularVelocity;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Cannot change object direction", ex);
            }
        }
    }
}