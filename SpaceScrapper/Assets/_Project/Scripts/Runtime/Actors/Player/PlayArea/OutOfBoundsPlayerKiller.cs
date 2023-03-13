using System;
using UnityEngine;

namespace Wokarol.SpaceScrapper.Actors
{
    public class OutOfBoundsPlayerKiller : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private float timeUntilKill = 3;

        public float KillCountdown { get; private set; }
        public bool IsOutOfBounds { get; private set; }
        public float NormalizedKillCountdown => KillCountdown / timeUntilKill;

        private void OnValidate()
        {
            if (player == null)
                TryGetComponent(out player);

            KillCountdown = timeUntilKill;
        }

        private void Update()
        {
            if (PlayerIsOutOfBounds())
            {
                IsOutOfBounds = true;
                KillCountdown -= Time.deltaTime;

                if (KillCountdown <= 0)
                {
                    player.Kill();
                }
            }
            else
            {
                IsOutOfBounds = false;
                KillCountdown = timeUntilKill;
            }
        }

        private bool PlayerIsOutOfBounds()
        {
            var allBounds = PlayAreaBounds.AllBounds;

            // We assume that if there are no bounds, player cannot be outside of them
            if (allBounds.Count == 0) return false;

            foreach (var bounds in allBounds)
            {
                if (bounds.IsWithinBounds(transform.position))
                    return false;
            }

            return true;
        }
    }
}
