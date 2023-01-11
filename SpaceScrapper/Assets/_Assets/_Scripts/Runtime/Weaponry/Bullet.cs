using UnityEngine;
using Wokarol.SpaceScrapper.Pooling;

namespace Wokarol.SpaceScrapper.Weaponry
{
    public class Bullet : MonoBehaviour, IPoolable<Bullet>
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float lifespan = 10f;
        [SerializeField] private int damage = 1;
        [SerializeField] private LayerMask hitMask = -1;

        private float inheritedVelocity = 0;
        private float timeOfDeath = 0;

        public Bullet OriginalPrefab { get; set; }
        public BasicPool<Bullet> Pool { get; set; }

        internal void Init(Vector2 initialVelocity)
        {
            float velocityAlongForward = Vector2.Dot(transform.up, initialVelocity);
            inheritedVelocity = velocityAlongForward;

            timeOfDeath = Time.time + lifespan;
        }

        private void FixedUpdate()
        {
            var velocity = (speed + inheritedVelocity) * transform.up;
            var distance = Time.deltaTime * velocity;
            bool queriesHitTriggersBefore = Physics2D.queriesHitTriggers;
            try
            {
                Physics2D.queriesHitTriggers = false;
                var hit = Physics2D.Raycast(transform.position, distance, distance.magnitude, hitMask);

                if (hit.collider != null)
                {
                    TryToHit(hit, velocity);

                    Pool.Return(this);
                    return;
                }

                transform.position += distance;
            }
            finally
            {
                Physics2D.queriesHitTriggers = queriesHitTriggersBefore;
            }

            if (Time.time > timeOfDeath)
            {
                Pool.Return(this);
                return;
            }
        }

        private void TryToHit(RaycastHit2D hit, Vector2 force)
        {
            IHittable[] hittables;
            if (hit.rigidbody != null)
            {
                hittables = hit.rigidbody.GetComponents<IHittable>();
            }
            else
            {
                hittables = hit.collider.GetComponents<IHittable>();
            }

            if (hittables != null)
            {
                for (int i = 0; i < hittables.Length; i++)
                {
                    hittables[i].Hit(force, hit.normal, hit.point, damage);
                }
            }
        }
    }
}
