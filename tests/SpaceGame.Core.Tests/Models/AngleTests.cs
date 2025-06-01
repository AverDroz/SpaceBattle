namespace SpaceGame.Core.Tests.Models;

public class AngleTests
{
    [Fact]
    public void Addition_Of_Angles_Should_Return_Correct_Result()
    {
        // Arrange
        var angle1 = new Angle(5, 360);
        var angle2 = new Angle(7, 360);

        // Act
        var result = angle1 + angle2;

        // Assert
        var expected = new Angle(12, 360);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Equals_Method_Should_Return_True_For_Equal_Angles()
    {
        // Arrange
        var angle1 = new Angle(15, 360);
        var angle2 = new Angle(23, 360);

        // Act & Assert
        Assert.True(angle1.Equals(angle2));
    }

    [Fact]
    public void Equality_Operator_Should_Return_True_For_Equal_Angles()
    {
        // Arrange
        var angle1 = new Angle(15, 360);
        var angle2 = new Angle(23, 360);

        // Act & Assert
        Assert.True(angle1 == angle2);
    }

    [Fact]
    public void Equals_Method_Should_Return_False_For_Different_Angles()
    {
        // Arrange
        var angle1 = new Angle(1, 360);
        var angle2 = new Angle(2, 360);

        // Act & Assert
        Assert.False(angle1.Equals(angle2));
    }

    [Fact]
    public void Inequality_Operator_Should_Return_True_For_Different_Angles()
    {
        // Arrange
        var angle1 = new Angle(1, 360);
        var angle2 = new Angle(2, 360);

        // Act & Assert
        Assert.True(angle1 != angle2);
    }

    [Fact]
    public void Angle_Should_Have_HashCode()
    {
        // Arrange
        var angle = new Angle(1, 360);

        // Act
        var hashCode = angle.GetHashCode();

        // Assert
        Assert.IsType<int>(hashCode);
    }

    [Fact]
    public void Implicit_Conversion_To_Double_Should_Work()
    {
        // Arrange
        var angle = new Angle(90, 360); // 90 градусов

        // Act
        double radians = angle;

        // Assert
        Assert.Equal(Math.PI / 2, radians, 5);
    }
}