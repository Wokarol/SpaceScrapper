using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.SpaceScrapper.Weaponry;

namespace Wokarol.SpaceScrapper
{
    public class BreakablePlaceholerBox : MonoBehaviour, IHittable
    {
        [SerializeField] private int startingHealth = 10;

        private Rigidbody2D body;
        private float health;

        private void Awake()
        {
            TryGetComponent(out body);

            health = startingHealth;
        }

        public void Hit(Vector2 force, Vector2 point, int damage)
        {
            if (damage > 0)
            {
                health -= damage;
                if (body != null)
                {
                    body.AddForceAtPosition(force, point);
                }
            }

            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
