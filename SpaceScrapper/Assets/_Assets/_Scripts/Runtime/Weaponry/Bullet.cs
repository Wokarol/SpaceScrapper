using System;
using UnityEngine;

namespace Wokarol.SpaceScrapper.Player
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private LayerMask hitMask = -1;

        private float inheritedVelocity = 0;

        internal void Init(Vector2 initialVelocity)
        {
            float velocityAlongForward = Vector2.Dot(transform.up, initialVelocity);
            inheritedVelocity = velocityAlongForward;
        }

        private void FixedUpdate()
        {

            var distance = (speed + inheritedVelocity) * Time.deltaTime * transform.up;
            var hit = Physics2D.Raycast(transform.position, distance, distance.magnitude, hitMask);

            if (hit.collider != null)
            {
                Destroy(gameObject);
            }

            transform.position += distance;
        }
    }
}
