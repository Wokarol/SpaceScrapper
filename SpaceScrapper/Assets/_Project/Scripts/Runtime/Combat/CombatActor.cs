using UnityEngine;
using Wokarol.GameSystemsLocator;

namespace Wokarol.SpaceScrapper.Combat
{
    public class CombatActor : MonoBehaviour
    {
        [SerializeField] private Faction faction;
        [SerializeField, Tooltip("Bigger = More important")] private int priority = 0;

        public Faction Faction => faction;
        public int Priority => priority;

        private void OnEnable()
        {
            GameSystems.Get<TargetingManager>().AddActor(this);
        }

        private void OnDisable()
        {
            GameSystems.Get<TargetingManager>().RemoveActor(this);
        }
    }
}
