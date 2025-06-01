namespace SpaceGame.Core
{
    /// <summary>
    /// Interface for objects that can move
    /// </summary>
    public interface IMovingObject
    {
        Vector Position { get; set; }
        Vector Velocity { get; }
    }

    /// <summary>
    /// Interface for objects that can rotate
    /// </summary>
    public interface IRotatingObject
    {
        Angle Direction { get; set; }
        Angle AngularVelocity { get; }
    }
}