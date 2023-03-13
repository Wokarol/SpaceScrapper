using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.SpaceScrapper.Combat
{
    public class TargetingManager : MonoBehaviour
    {
        [Header("Target weighting formula")]
        [SerializeField] private DistanceParams distanceCalculation;
        [SerializeField] private float priorityWeigth = 2;

        List<CombatActor> combatActors = new();

        public IReadOnlyList<CombatActor> AllActors => combatActors;

        public CombatActor GetBestTargetToFight(CombatActor caller, float? maxSeeingDistance = null)
        {
            // TODO: Consider adding checks to see if the new target is needed in a guard clause-like fashion (if target exists and is rather close, return it instead of searching more)
            // TODO: Check performance of that algorithm (stress test and stuff)

            CombatActor bestTarget = null;
            float bestTargetFit = float.NegativeInfinity;

            foreach (var candidate in combatActors)
            {
                if (candidate.Faction == caller.Faction) continue;

                float distance = Vector2.Distance(candidate.transform.position, caller.transform.position);
                int priority = candidate.Priority;

                if (maxSeeingDistance != null && distance > maxSeeingDistance) continue;

                float fit = distanceCalculation.CalculateFitForDistance(distance) + priority * priorityWeigth;

                if (fit > bestTargetFit)
                {
                    bestTargetFit = fit;
                    bestTarget = candidate;
                }
            }

            return bestTarget;
        }

        public void AddActor(CombatActor combatActor)
        {
            combatActors.Add(combatActor);
        }

        public void RemoveActor(CombatActor combatActor)
        {
            combatActors.Remove(combatActor);
        }

        [Serializable]
        private class DistanceParams
        {
            [SerializeField] private float closeDistance = 2;
            [SerializeField] private float farDistance = 5;
            [SerializeField] private float weigth = 5;

            public float CalculateFitForDistance(float distance)
            {
                return Mathf.InverseLerp(farDistance, closeDistance, distance) * weigth;
            }
        }
    }
}
