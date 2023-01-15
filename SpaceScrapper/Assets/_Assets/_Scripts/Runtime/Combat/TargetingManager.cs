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
            // TODO: Optimize this mess later
            IEnumerable<(CombatActor a, float d)> fittingTargets = actors
                .Where(a => a.Faction != caller.Faction)
                .OrderByDescending(a => a.Priority)
                .Select(a => (a, Vector2.Distance(a.transform.position, caller.transform.position)))
                .OrderBy(ad => ad.Item2);

            if (maxSeeingDistance != null)
            {
                fittingTargets = fittingTargets
                    .Where(ad => ad.d < maxSeeingDistance);
            }

            return fittingTargets // TODO: Improve target choice algorithm
                .FirstOrDefault().a;
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
