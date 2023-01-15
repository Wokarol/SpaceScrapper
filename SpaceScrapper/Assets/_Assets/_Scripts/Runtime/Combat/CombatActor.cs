using UnityEngine;
using Wokarol.GameSystemsLocator;

namespace Wokarol.SpaceScrapper.Combat
{
    public class CombatActor : MonoBehaviour
    {
        [SerializeField] private Faction faction;
        [SerializeField, Tooltip("Bigger = More important")] private int priority = 0;

        TargetingManager manager;
        bool isAdded = false;
        bool startPassed = false;

        public Faction Faction => faction;
        public int Priority => priority;

        private void Start()
        {
            startPassed = true;
            manager = GameSystems.Get<TargetingManager>();

            RegisterMyself();
        }

        private void OnEnable()
        {
            if (startPassed && !isAdded) RegisterMyself();
        }

        private void OnDisable()
        {
            if (startPassed && isAdded) RemoveMyself();
        }

        private void OnDestroy()
        {
            if (startPassed && isAdded) RemoveMyself();
        }

        private void RegisterMyself()
        {
            isAdded = true;
            manager.AddActor(this);
        }

        private void RemoveMyself()
        {
            isAdded = false;
            manager.RemoveActor(this);
        }
    }
}
