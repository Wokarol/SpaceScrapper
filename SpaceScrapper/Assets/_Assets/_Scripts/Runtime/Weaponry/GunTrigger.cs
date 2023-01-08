using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.SpaceScrapper.Player
{
    public class GunTrigger : MonoBehaviour
    {
        [SerializeField] private List<Gun> guns = new();

        public void UpdateShooting(bool wantsToShoot, Vector2 velocity)
        {
            for (int i = 0; i < guns.Count; i++)
            {
                guns[i].UpdateShooting(wantsToShoot, velocity);
            }
        }
    }
}
