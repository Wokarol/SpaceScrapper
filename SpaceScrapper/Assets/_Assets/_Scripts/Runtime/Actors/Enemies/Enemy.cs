using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
        [SerializeField] private NavMeshAgent agent = null;
        [SerializeField] private GunTrigger guns = null;
        [Header("Settings")]
        [SerializeField] private ShipMovementParams movementParams = new();
        [SerializeField] private int startingHealth = 10;
        [SerializeField] private LayerMask raycastBlockingMask = 0;
        [SerializeField] private LayerMask shootingTargetMask = 0;
        [SerializeField] private float seeingDistance = 6;
        [SerializeField] private float targetVelocityInfluence = 1.1f;

        private MovementValues lastMovementValues;
        private int health;

        private SceneContext sceneContext;

        public event Action Died;

        private void Awake()
        {
            health = startingHealth;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.updatePosition = false;
        }

        private void Start()
        {
            sceneContext = GameSystems.Get<SceneContext>();
        }

        private void Update()
        {
            engineController.UpdateThrusterAnimation(lastMovementValues.ThrustVector);

            var player = sceneContext.Player;
            var playerPosition = player.transform.position;

            Physics2D.queriesHitTriggers = false;
            var hit = Physics2D.Raycast(transform.position, playerPosition - transform.position, seeingDistance, raycastBlockingMask | shootingTargetMask);

            Debug.DrawRay(transform.position, (playerPosition - transform.position).normalized * seeingDistance);

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
            var player = sceneContext.Player;
            var playerPosition = player.transform.position;
            float distanceToPlayer = Vector2.Distance(playerPosition, transform.position);

            agent.SetDestination(playerPosition);

            var desiredVelocity = agent.desiredVelocity;
            agent.velocity = Vector3.zero;

            var direction = Vector2.ClampMagnitude(desiredVelocity, 1f);
            var aimPoint = playerPosition;

            float bulletFlyTime = distanceToPlayer / guns.CalculateAverageBulletSpeed();

            aimPoint += bulletFlyTime * targetVelocityInfluence * player.Velocity;
            var aimDiff = aimPoint - transform.position;

            if (distanceToPlayer > seeingDistance)
            {
                aimDiff = direction.normalized;
            }

            lastMovementValues = spaceshipController.HandleMovement(new InputValues(direction, aimDiff.normalized), movementParams);

            agent.nextPosition = transform.position;
            agent.velocity = spaceshipController.Velocity;
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
                Died?.Invoke();
            }
        }
    }
}
