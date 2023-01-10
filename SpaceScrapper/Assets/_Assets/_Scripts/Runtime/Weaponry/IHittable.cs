using UnityEngine;

namespace Wokarol.SpaceScrapper.Weaponry
{
    public interface IHittable
    {
        void Hit(Vector2 force, Vector2 point, int damage);
    }
}
