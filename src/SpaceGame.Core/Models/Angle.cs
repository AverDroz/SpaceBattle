using System;

namespace SpaceGame.Core
{
    /// <summary>
    /// Angle represented as rational number (Task 1, Point 7)
    /// </summary>
    public class Angle
    {
        private readonly int _numerator;
        private static readonly int _denominator = 360; // Static denominator as per requirements

        /// <summary>
        /// Creates angle with specified numerator and denominator
        /// </summary>
        /// <param name="numerator">Numerator</param>
        /// <param name="denominator">Denominator (must match static denominator)</param>
        public Angle(int numerator, int denominator)
        {
            if (denominator != _denominator)
                throw new ArgumentException($"Denominator must be {_denominator}");

            // Normalize angle to [0, denominator) range
            _numerator = ((numerator % _denominator) + _denominator) % _denominator;
        }

        /// <summary>
        /// Gets numerator
        /// </summary>
        public int Numerator => _numerator;

        /// <summary>
        /// Gets denominator
        /// </summary>
        public static int Denominator => _denominator;

        /// <summary>
        /// Angle addition
        /// </summary>
        public static Angle operator +(Angle left, Angle right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            int sum = left._numerator + right._numerator;
            return new Angle(sum, _denominator);
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(Angle left, Angle right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(Angle left, Angle right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implicit conversion to double for Math operations
        /// </summary>
        public static implicit operator double(Angle angle)
        {
            return angle._numerator * 2.0 * Math.PI / _denominator;
        }

        /// <summary>
        /// Checks equality with another angle
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not Angle other) return false;
            return _numerator == other._numerator;
        }

        /// <summary>
        /// Gets hash code
        /// </summary>
        public override int GetHashCode()
        {
            return _numerator.GetHashCode();
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return $"{_numerator}/{_denominator}";
        }
    }
}