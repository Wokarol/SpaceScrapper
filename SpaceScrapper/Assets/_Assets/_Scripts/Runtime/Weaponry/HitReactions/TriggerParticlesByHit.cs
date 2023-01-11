using UnityEngine;

namespace Wokarol.SpaceScrapper.Weaponry
{
    public class TriggerParticlesByHit : MonoBehaviour, IHittable
    {
        [SerializeField] private ParticleSystem system;

        public void Hit(Vector2 force, Vector2 normal, Vector2 point, int damage)
        {
            var particleDirection = Vector2.Reflect(force, normal);

            system.transform.SetPositionAndRotation(point, Quaternion.FromToRotation(Vector2.up, particleDirection));
            system.Play();
        }
    }
}
