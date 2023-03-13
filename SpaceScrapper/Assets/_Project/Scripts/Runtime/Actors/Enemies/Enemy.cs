using System;
using UnityEngine;
using UnityEngine.AI;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Actors.Common;
using Wokarol.SpaceScrapper.Actors.DataTypes;
using Wokarol.SpaceScrapper.Combat;
using Wokarol.SpaceScrapper.Weaponry;

namespace Wokarol.SpaceScrapper.Actors
{
    public class Enemy : MonoBehaviour, IHittable
    {
        [Header("Object References")]
        [SerializeField] private SpaceshipController spaceshipController = null;
        [SerializeField] private MultiDirectionalEngineVisualController engineController = null;
        [SerializeField] private NavMeshAgent agent = null;
        [SerializeField] private GunTrigger guns = null;
        [SerializeField] private CombatActor combatActor = null;
        [Header("Settings")]
        [SerializeField] private ShipMovementParams movementParams = new();
        [SerializeField] private int startingHealth = 10;
        [SerializeField] private LayerMask raycastBlockingMask = 0;
        [SerializeField] private LayerMask shootingTargetMask = 0;
        [SerializeField] private float seeingDistance = 6;
        [SerializeField] private float targetVelocityInfluence = 1.1f;
        [SerializeField] private float maxDeviationFromStraightAim = 45f;

        private MovementValues lastMovementValues;
        private int health;
        private TargetingManager targetingManager;

        private CombatTarget currentTarget = default;

        public event Action Died;
        private bool alreadyDiedAndShouldNotBeAbleToDieAgain = false;

        private void Awake()
        {
            health = startingHealth;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.updatePosition = false;
        }

        private void Start()
        {
            targetingManager = GameSystems.Get<TargetingManager>();

            if (spaceshipController == null) Debug.LogError("There is no spaceship controller assigned, that can cause problems in the future");
            if (engineController == null) Debug.LogError("There is no engine controller assigned, that can cause problems in the future");
            if (agent == null) Debug.LogError("There is no agent assigned, that can cause problems in the future");
            if (guns == null) Debug.LogError("There are no guns assigned, that can cause problems in the future");
            if (combatActor == null) Debug.LogError("There is no combat actor assigned, that can cause problems in the future");
        }

        private void Update()
        {
            UpdateTarget();
            engineController.UpdateThrusterAnimation(lastMovementValues.ThrustVector);
            UpdateEnemyLogic();
        }

        private void UpdateTarget()
        {
            var target = targetingManager.GetBestTargetToFight(combatActor);

            if (target == null)
            {
                currentTarget = CombatTarget.None;
                return;
            }

            currentTarget = CombatTarget.CreateOrReuse(target.transform, currentTarget);
        }

        private void UpdateEnemyLogic()
        {
            if (currentTarget.Exists)
            {
                var vectorTowardsTarget = currentTarget.Position - transform.position;

                Physics2D.queriesHitTriggers = false;
                var hit = Physics2D.Raycast(transform.position, vectorTowardsTarget, seeingDistance, raycastBlockingMask | shootingTargetMask);

                bool wantsToShoot = CheckIfIWantToShootTarget(hit);
                guns.UpdateShooting(wantsToShoot, new(Vector2.zero));
            }
            else
            {
                guns.UpdateShooting(false, new(Vector2.zero));
            }
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
            if (currentTarget.Exists)
            {
                var targetPosition = currentTarget.Position;
                float distanceToTarget = Vector2.Distance(targetPosition, transform.position);

                agent.SetDestination(targetPosition);
                agent.isStopped = false;

                var desiredVelocity = agent.desiredVelocity;
                agent.velocity = Vector3.zero;

                var direction = Vector2.ClampMagnitude(desiredVelocity, 1f);

                var aimDiff = distanceToTarget > seeingDistance
                    ? direction.normalized
                    : CalculateAimWithVelocity(targetPosition, currentTarget.Velocity);

                lastMovementValues = spaceshipController.HandleMovement(new InputValues(direction, aimDiff.normalized), movementParams);
            }
            else
            {
                agent.isStopped = true;

                lastMovementValues = spaceshipController.HandleMovement(new InputValues(Vector2.zero, spaceshipController.Forward), movementParams);
            }

            agent.nextPosition = transform.position;
            agent.velocity = spaceshipController.Velocity;
        }

        private Vector2 CalculateAimWithVelocity(Vector3 aimPoint, Vector3 targetVelocity)
        {
            // TODO: Improve aiming algorithm
            float distanceToAimPoint = Vector2.Distance(aimPoint, transform.position);
            float bulletFlyTime = distanceToAimPoint / guns.CalculateAverageBulletSpeed();

            var straightAim = aimPoint - transform.position;

            aimPoint += bulletFlyTime * targetVelocityInfluence * targetVelocity;
            var aimDiff = aimPoint - transform.position;

            float angle = Vector2.SignedAngle(straightAim, aimDiff);

            if (Mathf.Abs(angle) > maxDeviationFromStraightAim)
            {
                aimDiff = Quaternion.AngleAxis(maxDeviationFromStraightAim * Mathf.Sign(angle), Vector3.forward) * straightAim;
            }

            return aimDiff;
        }

        public void Hit(Vector2 force, Vector2 normal, Vector2 point, int damage)
        {
            if (damage > 0)
            {
                health -= damage;
            }

            if (health <= 0 && !alreadyDiedAndShouldNotBeAbleToDieAgain)
            {
                Destroy(gameObject);
                alreadyDiedAndShouldNotBeAbleToDieAgain = true;
                Died?.Invoke();
            }
        }
        private void OnDrawGizmos()
        {
            if (!currentTarget.Exists) return;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTarget.Transform.position);
        }
    }
}
