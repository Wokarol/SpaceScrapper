using System;
using UnityEngine;

namespace Wokarol.SpaceScrapper.Player
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] private ShootingMode mode = ShootingMode.Automatic;
        [SerializeField] private float delayBetweenShots = 0.1f;
        [SerializeField] private Bullet bulletPrefab = null;

        bool wantedToShotBefore = false;
        float lastShotTime = float.NegativeInfinity;


        public void UpdateShooting(bool wantsToShoot, Vector2 velocity)
        {
            bool lastShootWasLongAgo = (Time.time - lastShotTime) > delayBetweenShots;
            switch (mode)
            {
                case ShootingMode.Automatic:
                    if (wantsToShoot && lastShootWasLongAgo)
                    {
                        Shoot(velocity);
                    }
                    break;

                case ShootingMode.SemiAutomatic:
                    if (!wantedToShotBefore && wantsToShoot && lastShootWasLongAgo)
                    {
                        Shoot(velocity);
                    }
                    break;
            }
            wantedToShotBefore = wantsToShoot;
        }

        private void Shoot(Vector2 velocity)
        {
            var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.Init(velocity);
            lastShotTime = Time.time;
        }

        enum ShootingMode
        {
            Automatic,
            SemiAutomatic,
        }
    }
}
