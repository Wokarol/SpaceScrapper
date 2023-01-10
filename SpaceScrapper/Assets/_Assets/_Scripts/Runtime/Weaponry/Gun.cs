using System;
using UnityEngine;
using Wokarol.SpaceScrapper.Pooling;

namespace Wokarol.SpaceScrapper.Weaponry
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] private ShootingMode mode = ShootingMode.Automatic;
        [SerializeField] private float delayBetweenShots = 0.1f;
        [SerializeField] private Bullet bulletPrefab = null;
        [SerializeField] private BasicPool<Bullet> bulletPool = null;

        bool wantedToShotBefore = false;
        float lastShotTime = float.NegativeInfinity;


        public void UpdateShooting(bool wantsToShoot, ShootParams shootParams)
        {
            bool lastShootWasLongAgo = (Time.time - lastShotTime) > delayBetweenShots;
            switch (mode)
            {
                case ShootingMode.Automatic:
                    if (wantsToShoot && lastShootWasLongAgo)
                    {
                        Shoot(shootParams);
                    }
                    break;

                case ShootingMode.SemiAutomatic:
                    if (!wantedToShotBefore && wantsToShoot && lastShootWasLongAgo)
                    {
                        Shoot(shootParams);
                    }
                    break;
            }
            wantedToShotBefore = wantsToShoot;
        }

        private void Shoot(ShootParams shootParams)
        {
            var bullet = bulletPool.Get(bulletPrefab, transform.position, transform.rotation);
            bullet.Init(shootParams.velocity);
            lastShotTime = Time.time;
        }

        enum ShootingMode
        {
            Automatic,
            SemiAutomatic,
        }
    }
}
