using UnityEngine;

namespace Wokarol.SpaceScrapper.Weaponry
{
    public class TriggerParticlesByHit : MonoBehaviour, IHittable
    {
        [SerializeField] private ParticleSystem system;

        [Header("Optional")]
        [SerializeField] private Collider2D accurateCollider = null;
        [SerializeField] private float accurateColliderOffset = 0.4f;
        [SerializeField] private float raycastDeepness = 1f;

        RaycastHit2D[] hitsCached;

        public void Hit(Vector2 force, Vector2 normal, Vector2 point, int damage)
        {
            if (accurateCollider != null)
            {
                hitsCached ??= new RaycastHit2D[5];

                var rayOrigin = point - force.normalized * accurateColliderOffset;

                bool queriesHitCollidersBefore = Physics2D.queriesHitTriggers;
                try
                {
                    Physics2D.queriesHitTriggers = true;
                    int hitCount = Physics2D.RaycastNonAlloc(rayOrigin, force.normalized, hitsCached, raycastDeepness, 1 << accurateCollider.gameObject.layer);

                    if (hitCount <= 0)
                    {
                        Debug.LogWarning("Found no hits, try increasing the collider offset", this);
                    }

                    bool foundCollider = false;
                    for (int i = 0; i < hitCount; i++)
                    {
                        if (hitsCached[i].collider == accurateCollider)
                        {
                            normal = hitsCached[i].normal;
                            point = hitsCached[i].point;
                            foundCollider = true;
                            break;
                        }
                    }
                    if (!foundCollider)
                    {
                        Debug.LogWarning("Hits did not contain the collider", this);
                    }
                }
                finally
                {
                    Physics2D.queriesHitTriggers = queriesHitCollidersBefore;
                }
            }

            var particleDirection = Vector2.Reflect(force, normal);

            system.transform.SetPositionAndRotation(point, Quaternion.FromToRotation(Vector2.up, particleDirection));
            system.Play();
        }

        private void OnDrawGizmosSelected()
        {
            if (accurateCollider != null)
            {
                var point = accurateCollider.ClosestPoint(Vector3.up * 1000f + accurateCollider.transform.position);
                var direction = Vector2.up;
                var perp = Vector2.right;

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(point, point + direction * accurateColliderOffset);
            }
        }
    }
}
