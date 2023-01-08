using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.SpaceScrapper.Weaponry
{
    public class GunTrigger : MonoBehaviour
    {
        [SerializeField] private List<Gun> guns = new();

        public void UpdateShooting(bool wantsToShoot, ShootParams shootParams)
        {
            for (int i = 0; i < guns.Count; i++)
            {
                guns[i].UpdateShooting(wantsToShoot, shootParams);
            }
        }
    }

    public struct ShootParams
    {
        public readonly Vector2 velocity;

        public ShootParams(Vector2 velocity)
        {
            this.velocity = velocity;
        }
    }
}
