using System;
using UnityEngine;
using Wokarol.SpaceScrapper.Weaponry;

namespace Wokarol.SpaceScrapper.Actors
{
    public class BaseCore : MonoBehaviour, IHittable
    {
        [SerializeField] private int maxHealth = 200;

        private int health;

        public int Health => health;
        public int MaxHealth => maxHealth;

        public event Action Destroyed;

        private void Awake()
        {
            health = maxHealth;
        }

        public void Hit(Vector2 force, Vector2 normal, Vector2 point, int damage)
        {
            if (damage <= 0) return;

            health -= damage;

            if (health <= 0)
            {
                Destroyed?.Invoke();
                Destroy(gameObject);
            }
        }

        public class Memento
        {
            public int health;

            public static Memento CreateFrom(BaseCore core)
            {
                return new Memento()
                {
                    health = core.health,
                };
            }

            public void InjectInto(BaseCore core)
            {
                core.health = health;
            }
        }
    }
}
