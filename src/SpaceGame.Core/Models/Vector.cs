namespace SpaceGame.Core.Models;

public class Vector : IEquatable<Vector>
{
    private readonly int[] _coordinates;

    public Vector(params int[] coordinates)
    {
        _coordinates = coordinates ?? throw new ArgumentNullException(nameof(coordinates));
        if (_coordinates.Length == 0)
            throw new ArgumentException("Vector must have at least one coordinate", nameof(coordinates));
    }

    public int Dimension => _coordinates.Length;

    public int this[int index] => _coordinates[index];

    public static Vector operator +(Vector left, Vector right)
    {
        if (left == null) throw new ArgumentNullException(nameof(left));
        if (right == null) throw new ArgumentNullException(nameof(right));
        
        if (left.Dimension != right.Dimension)
            throw new ArgumentException($"Cannot add vectors of different dimensions: {left.Dimension} and {right.Dimension}");

        var result = new int[left.Dimension];
        for (int i = 0; i < left.Dimension; i++)
        {
            result[i] = left._coordinates[i] + right._coordinates[i];
        }

        return new Vector(result);
    }

    public bool Equals(Vector? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Dimension != other.Dimension) return false;

        for (int i = 0; i < Dimension; i++)
        {
            if (_coordinates[i] != other._coordinates[i])
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Vector);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            foreach (var coordinate in _coordinates)
            {
                hash = hash * 23 + coordinate.GetHashCode();
            }
            return hash;
        }
    }

    public static bool operator ==(Vector? left, Vector? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Vector? left, Vector? right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"({string.Join(", ", _coordinates)})";
    }
}