using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wokarol.SpaceScrapper.Combat
{
    public class TargetingManager : MonoBehaviour
    {
        List<CombatActor> actors = new();

        public IReadOnlyList<CombatActor> AllActors => actors;

        public CombatActor GetBestTargetToFight(CombatActor caller, float? maxSeeingDistance = null)
        {
            // TODO: Redesign the algorithm
            // 1. Remove Linq in favour of manual looping
            // 2. Make priority a weight instead of hard sort (distance * a  +  priority * b  +  ...)
            // 3. Consider adding checks to see if the new target is needed in a guard clause-like fashion (if target exists and is rather close, return it instead of searching more)

            // TODO: Check performance of that algorithm (stress test and stuff)

            // TODO: Optimize this mess later
            IEnumerable<(CombatActor a, float d)> fittingTargets = actors
                .Where(a => a.Faction != caller.Faction)
                .OrderByDescending(a => a.Priority)
                .Select(a => (a, Vector2.Distance(a.transform.position, caller.transform.position)))
                .OrderBy(ad => ad.Item2);

            if (maxSeeingDistance != null)
            {
                // That should probably be done first to limit the search space
                fittingTargets = fittingTargets
                    .Where(ad => ad.d < maxSeeingDistance);
            }

            return fittingTargets // TODO: Improve target choice algorithm
                .FirstOrDefault().a; // The f..ck is "a"
        }

        public void AddActor(CombatActor combatActor)
        {
            actors.Add(combatActor);
        }

        public void RemoveActor(CombatActor combatActor)
        {
            actors.Remove(combatActor);
        }
    }
}
