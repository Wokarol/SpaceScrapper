using UnityEngine;

namespace Wokarol.SpaceScrapper.Actors
{
    public interface IHasVelocity
    {
        Vector3 Velocity { get; }
    }
}