using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.SpaceScrapper.Actors.Common;
using Wokarol.SpaceScrapper.Weaponry;

namespace Wokarol.SpaceScrapper
{
    public class Enemy : MonoBehaviour, IHittable
    {
        [Header("Object References")]
        [SerializeField] private SpaceshipController spaceshipController = null;
        [SerializeField] private MultiDirectionalEngineVisualController engineController = null;
        [SerializeField] private GunTrigger guns = null;
        [Header("Settings")]
        [SerializeField] private ShipMovementParams movementParams = new();
        [SerializeField] private int startingHealth = 10;

        private MovementValues lastMovementValues;
        private int health;

        private void Awake()
        {
            health = startingHealth;
        }

        private void Update()
        {
            engineController.UpdateThrusterAnimation(lastMovementValues.ThrustVector);
            guns.UpdateShooting(false, new(Vector2.zero));

        }

        private void FixedUpdate()
        {
            var direction = Quaternion.Euler(0, 0, -30f) * transform.up;
            //lastMovementValues = spaceshipController.HandleMovement(new InputValues(Vector2.up, direction), movementParams);
        }

        public void Hit(Vector2 force, Vector2 normal, Vector2 point, int damage)
        {
            if (damage > 0)
            {
                health -= damage;
            }

            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
