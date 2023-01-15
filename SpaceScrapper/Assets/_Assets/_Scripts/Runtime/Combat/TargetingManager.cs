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

        public CombatActor GetBestTargetToFight(CombatActor caller)
        {
            // TODO: Optimize this mess later
            return actors
                .Where(a => a.Faction != caller.Faction)
                .OrderBy(a => a.Priority) // TODO: Improve target choice algorithm
                .FirstOrDefault();
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
