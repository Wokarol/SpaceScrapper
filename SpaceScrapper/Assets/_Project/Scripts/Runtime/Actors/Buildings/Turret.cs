using UnityEngine;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Actors.DataTypes;
using Wokarol.SpaceScrapper.Combat;
using Wokarol.SpaceScrapper.Saving;
using Wokarol.SpaceScrapper.Weaponry;

namespace Wokarol.SpaceScrapper.Actors
{
    public class Turret : MonoBehaviour, IHittable, IPersistentActorStateSource
    {
        [SerializeField] private Transform turretHead = null;
        [SerializeField] private GunTrigger gunTrigger = null;
        [SerializeField] private CombatActor combatActor = null;
        [Space]
        [SerializeField] private int startingHealth = 10;
        [SerializeField] private LayerMask raycastBlockingMask = 0;
        [SerializeField] private LayerMask shootingTargetMask = 0;
        [SerializeField] private float trackingSmoothing = 0.2f;
        [SerializeField] private float seeingDistance = 6;
        [SerializeField] private float targetVelocityInfluence = 1.1f;

        private CombatTarget currentTarget = default;
        private float turretHeadVelocity = 0;
        private TargetingManager targetingManager;
        private int health;

        private void Start()
        {
            health = startingHealth;
            targetingManager = GameSystems.Get<TargetingManager>();
        }

        private void Update()
        {
            UpdateTarget();
            UpdateTurretHead();
            UpdateTurretLogic();
        }

        private void UpdateTurretLogic()
        {
            if (currentTarget.Exists)
            {
                var vectorTowardsTarget = currentTarget.Position - transform.position;

                Physics2D.queriesHitTriggers = false;
                var hit = Physics2D.Raycast(transform.position, vectorTowardsTarget, seeingDistance, raycastBlockingMask | shootingTargetMask);

                bool wantsToShoot = CheckIfIWantToShootTarget(hit);
                gunTrigger.UpdateShooting(wantsToShoot, new(Vector2.zero));
            }
            else
            {
                gunTrigger.UpdateShooting(false, new(Vector2.zero));
            }
        }

        private void UpdateTurretHead()
        {
            if (currentTarget.Exists)
            {
                var targetPosition = currentTarget.Position;
                float distanceToTarget = Vector2.Distance(targetPosition, transform.position);

                var aimDiff = CalculateAimWithVelocity(targetPosition, currentTarget.Velocity);
                var aimDirection = Vector2.SignedAngle(Vector2.up, aimDiff);

                var rotation = turretHead.transform.eulerAngles;
                rotation.z = Mathf.SmoothDampAngle(rotation.z, aimDirection, ref turretHeadVelocity, trackingSmoothing);
                turretHead.transform.eulerAngles = rotation;
            }
            else
            {
                var rotation = turretHead.transform.eulerAngles;
                rotation.z = Mathf.SmoothDampAngle(rotation.z, rotation.z, ref turretHeadVelocity, trackingSmoothing);
                turretHead.transform.eulerAngles = rotation;
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

        private Vector2 CalculateAimWithVelocity(Vector3 aimPoint, Vector3 targetVelocity)
        {
            // TODO: Improve aiming algorithm
            float distanceToAimPoint = Vector2.Distance(aimPoint, transform.position);
            float bulletFlyTime = distanceToAimPoint / gunTrigger.CalculateAverageBulletSpeed();

            aimPoint += bulletFlyTime * targetVelocityInfluence * targetVelocity;
            var aimDiff = aimPoint - transform.position;
            return aimDiff;
        }

        private void UpdateTarget()
        {
            var target = targetingManager.GetBestTargetToFight(combatActor, seeingDistance);

            if (target == null)
            {
                currentTarget = CombatTarget.None;
                return;
            }

            currentTarget = CombatTarget.CreateOrReuse(target.transform, currentTarget);
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

        public void SaveState(PersistentActorStateWriter writer)
        {
            writer.Write("turret-state", new TurretMemento()
            {
                health = health,
            });
        }

        public void LoadState(PersistentActorStateReader reader)
        {
            var memento = reader.Read<TurretMemento>("turret-state");
            health = memento.health;
        }

        class TurretMemento
        {
            public int health;
        }
    }
}
