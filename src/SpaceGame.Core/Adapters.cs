using System;
using System.Collections.Generic;

namespace SpaceGame.Core
{
    /// <summary>
    /// Adapter for IMovingObject interface
    /// </summary>
    public class MovingObjectAdapter : IMovingObject
    {
        private readonly IDictionary<string, object> _gameObject;

        public MovingObjectAdapter(IDictionary<string, object> gameObject)
        {
            _gameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
        }

        public Vector Position
        {
            get
            {
                if (_gameObject.TryGetValue("Position", out var position) && position is Vector vector)
                    return vector;
                throw new InvalidOperationException("Cannot get Position from game object");
            }
            set
            {
                _gameObject["Position"] = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public Vector Velocity
        {
            get
            {
                if (_gameObject.TryGetValue("Velocity", out var velocity) && velocity is Vector vector)
                    return vector;
                throw new InvalidOperationException("Cannot get Velocity from game object");
            }
        }
    }

    /// <summary>
    /// Adapter for IRotatingObject interface
    /// </summary>
    public class RotatingObjectAdapter : IRotatingObject
    {
        private readonly IDictionary<string, object> _gameObject;

        public RotatingObjectAdapter(IDictionary<string, object> gameObject)
        {
            _gameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
        }

        public Angle Direction
        {
            get
            {
                if (_gameObject.TryGetValue("Direction", out var direction) && direction is Angle angle)
                    return angle;
                throw new InvalidOperationException("Cannot get Direction from game object");
            }
            set
            {
                _gameObject["Direction"] = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public Angle AngularVelocity
        {
            get
            {
                if (_gameObject.TryGetValue("AngularVelocity", out var velocity) && velocity is Angle angle)
                    return angle;
                throw new InvalidOperationException("Cannot get AngularVelocity from game object");
            }
        }
    }
}