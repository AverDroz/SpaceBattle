using SpaceGame.Core.Models;

namespace SpaceGame.Core.Interfaces;

public interface IMovingObject
{
    Vector Location { get; set; }
    Vector Velocity { get; }
}