using System;
using System.Linq;

namespace SpaceGame.Core
{
    /// <summary>
    /// Vector in n-dimensional space with integer coordinates 
    /// </summary>
    public class Vector
    {
        private readonly int[] _coordinates;

        /// <summary>
        /// Creates vector with specified coordinates
        /// </summary>
        /// <param name="coordinates">Coordinates array</param>
        public Vector(params int[] coordinates)
        {
            _coordinates = coordinates?.ToArray() ?? throw new ArgumentNullException(nameof(coordinates));
        }

        /// <summary>
        /// Gets coordinate at specified index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Coordinate value</returns>
        public int this[int index] => _coordinates[index];

        /// <summary>
        /// Gets vector dimension
        /// </summary>
        public int Dimension => _coordinates.Length;

        /// <summary>
        /// Vector addition operation
        /// </summary>
        /// <param name="left">First vector</param>
        /// <param name="right">Second vector</param>
        /// <returns>Sum of vectors</returns>
        /// <exception cref="ArgumentException">Thrown when vectors have different dimensions</exception>
        public static Vector operator +(Vector left, Vector right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            if (left.Dimension != right.Dimension)
                throw new ArgumentException("Vectors must have the same dimension");

            var result = new int[left.Dimension];
            for (int i = 0; i < left.Dimension; i++)
            {
                result[i] = left[i] + right[i];
            }

            return new Vector(result);
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(Vector left, Vector right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(Vector left, Vector right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Checks equality with another vector
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not Vector other) return false;
            if (Dimension != other.Dimension) return false;

            for (int i = 0; i < Dimension; i++)
            {
                if (_coordinates[i] != other._coordinates[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets hash code
        /// </summary>
        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var coord in _coordinates)
            {
                hash.Add(coord);
            }
            return hash.ToHashCode();
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return $"({string.Join(", ", _coordinates)})";
        }
    }
}