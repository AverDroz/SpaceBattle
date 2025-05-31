namespace SpaceGame.Core.Models;

public class Angle : IEquatable<Angle>
{
    private readonly int _numerator;
    private static readonly int Denominator = 360;

    public Angle(int numerator, int denominator)
    {
        if (denominator != Denominator)
        {
            // Приводим к общему знаменателю
            _numerator = numerator * Denominator / denominator;
        }
        else
        {
            _numerator = numerator;
        }
        
        // Нормализуем угол к диапазону [0, Denominator)
        _numerator = _numerator % Denominator;
        if (_numerator < 0) _numerator += Denominator;
    }

    public int Numerator => _numerator;
    public static int CommonDenominator => Denominator;

    public static Angle operator +(Angle left, Angle right)
    {
        if (left == null) throw new ArgumentNullException(nameof(left));
        if (right == null) throw new ArgumentNullException(nameof(right));

        var sum = left._numerator + right._numerator;
        return new Angle(sum, Denominator);
    }

    public static implicit operator double(Angle angle)
    {
        if (angle == null) throw new ArgumentNullException(nameof(angle));
        return 2 * Math.PI * angle._numerator / Denominator;
    }

    public bool Equals(Angle? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _numerator == other._numerator;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Angle);
    }

    public override int GetHashCode()
    {
        return _numerator.GetHashCode();
    }

    public static bool operator ==(Angle? left, Angle? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Angle? left, Angle? right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"{_numerator}/{Denominator}";
    }
}