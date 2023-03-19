using UnityEngine;
using Wokarol.SpaceScrapper.Weaponry;

namespace Wokarol.SpaceScrapper
{
    public class BreakablePlaceholerBox : MonoBehaviour, IHittable
    {
        [SerializeField] private int startingHealth = 10;
        [SerializeField] private ParticleSystem explosionParticles;

        private float health;

        private void Awake()
        {
            health = startingHealth;
        }

        public void Hit(Vector2 force, Vector2 normal, Vector2 point, int damage)
        {
            if (damage > 0)
            {
                health -= damage;
            }

            if (health <= 0)
            {
                explosionParticles.transform.parent = null;
                explosionParticles.Play();
                Destroy(gameObject);
            }
        }
    }
}
