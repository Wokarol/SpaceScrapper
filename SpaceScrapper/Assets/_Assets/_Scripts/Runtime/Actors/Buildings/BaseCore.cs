using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.SpaceScrapper.Weaponry;

namespace Wokarol.SpaceScrapper
{
    public class BaseCore : MonoBehaviour, IHittable
    {
        [SerializeField] private int maxHealth = 200;

        private int health;

        public int Health => health;
        public event Action Destoyed;

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
                Destoyed?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
