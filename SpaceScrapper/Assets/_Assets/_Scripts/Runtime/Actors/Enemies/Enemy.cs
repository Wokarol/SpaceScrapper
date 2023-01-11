using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;
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
        [SerializeField] private LayerMask raycastBlockingMask = 0;
        [SerializeField] private LayerMask shootingTargetMask = 0;
        [SerializeField] private float seeingDistance = 6;

        private MovementValues lastMovementValues;
        private int health;

        private SceneContext sceneContext;

        private void Awake()
        {
            health = startingHealth;
        }

        private void Start()
        {
            sceneContext = GameSystems.Get<SceneContext>();
        }

        private void Update()
        {
            engineController.UpdateThrusterAnimation(lastMovementValues.ThrustVector);

            var hit = Physics2D.Raycast(transform.position, spaceshipController.Forward, seeingDistance, raycastBlockingMask | shootingTargetMask);

            Debug.Log($"{hit.collider?.name ?? "Null"} {hit.rigidbody?.name ?? "Null"}");
            Debug.DrawRay(transform.position, spaceshipController.Forward * seeingDistance);

            bool wantsToShoot = CheckIfIWantToShootTarget(hit);

            guns.UpdateShooting(wantsToShoot, new(Vector2.zero));
        }

        private bool CheckIfIWantToShootTarget(RaycastHit2D hit)
        {
            if (hit.collider == null && hit.rigidbody == null) return false;

            var hitObject = hit.rigidbody != null
                ? hit.rigidbody.gameObject
                : (hit.collider ? hit.collider.gameObject : null);

            return ((1 << hitObject.layer) & shootingTargetMask) > 0;
        }

        private void FixedUpdate()
        {
            var playerPos = sceneContext.Player.transform.position;

            var direction = (playerPos - transform.position).normalized;
            lastMovementValues = spaceshipController.HandleMovement(new InputValues(direction, direction), movementParams);
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
