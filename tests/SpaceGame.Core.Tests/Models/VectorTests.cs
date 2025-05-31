namespace SpaceGame.Core.Tests.Models;

public class VectorTests
{
    [Fact]
    public void Addition_Of_Vectors_Should_Return_Correct_Result()
    {
        // Arrange
        var vector1 = new Vector(1, -1, 2);
        var vector2 = new Vector(-1, 1, -2);

        // Act
        var result = vector1 + vector2;

        // Assert
        Assert.Equal(new Vector(0, 0, 0), result);
    }

    [Fact]
    public void Addition_Of_Vectors_With_Different_Dimensions_Should_Throw_ArgumentException()
    {
        // Arrange
        var vector1 = new Vector(1, 2, 3);
        var vector2 = new Vector(1, 2);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => vector1 + vector2);
    }

    [Fact]
    public void Addition_Of_Vectors_With_Different_Dimensions_Reversed_Should_Throw_ArgumentException()
    {
        // Arrange
        var vector1 = new Vector(1, 2);
        var vector2 = new Vector(1, 2, 3);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => vector1 + vector2);
    }

    [Fact]
    public void Equals_Method_Should_Return_True_For_Equal_Vectors()
    {
        // Arrange
        var vector1 = new Vector(1, 2, 3);
        var vector2 = new Vector(1, 2, 3);

        // Act & Assert
        Assert.True(vector1.Equals(vector2));
    }

    [Fact]
    public void Equality_Operator_Should_Return_True_For_Equal_Vectors()
    {
        // Arrange
        var vector1 = new Vector(1, 2, 3);
        var vector2 = new Vector(1, 2, 3);

        // Act & Assert
        Assert.True(vector1 == vector2);
    }

    [Fact]
    public void Equals_Method_Should_Return_False_For_Different_Vectors()
    {
        // Arrange
        var vector1 = new Vector(1, 2, 3);
        var vector2 = new Vector(1, 2, 4);

        // Act & Assert
        Assert.False(vector1.Equals(vector2));
    }

    [Fact]
    public void Inequality_Operator_Should_Return_True_For_Different_Vectors()
    {
        // Arrange
        var vector1 = new Vector(1, 2, 3);
        var vector2 = new Vector(1, 2, 4);

        // Act & Assert
        Assert.True(vector1 != vector2);
    }

    [Fact]
    public void Vector_Should_Have_HashCode()
    {
        // Arrange
        var vector = new Vector(1, 2, 3);

        // Act
        var hashCode = vector.GetHashCode();

        // Assert
        Assert.IsType<int>(hashCode);
    }
}