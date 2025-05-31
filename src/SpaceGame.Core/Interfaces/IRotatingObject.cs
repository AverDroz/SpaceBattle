using SpaceGame.Core.Models;

namespace SpaceGame.Core.Interfaces;

public interface IRotatingObject
{
    Angle Direction { get; set; }
    Angle AngularVelocity { get; }
}