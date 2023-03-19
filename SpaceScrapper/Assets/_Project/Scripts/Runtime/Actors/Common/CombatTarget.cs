using UnityEngine;

namespace Wokarol.SpaceScrapper.Actors.DataTypes
{
    public readonly struct CombatTarget
    {
        public readonly Transform Transform;
        public readonly IHasVelocity HasVelocity;

        public Vector3 Position => Transform.position;
        public Vector3 Velocity => HasVelocity != null ? HasVelocity.Velocity : Vector3.zero;
        public bool Exists => Transform != null;

        private CombatTarget(Transform transform, IHasVelocity hasVelocity)
        {
            Transform = transform;
            HasVelocity = hasVelocity;
        }

        public static CombatTarget None => new CombatTarget();

        public static CombatTarget CreateOrReuse(Transform transform, CombatTarget last)
        {
            if (last.Transform == transform) return last;
            else return Create(transform);
        }
        public static CombatTarget Create(Transform transform)
        {
            return new CombatTarget(
                transform: transform,
                hasVelocity: transform.GetComponent<IHasVelocity>()
            );
        }
    }
}
