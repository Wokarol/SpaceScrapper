using UnityEngine;

namespace Wokarol.SpaceScrapper.Actors.Spawning
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private float spawnRadius = 16;

        public Vector2 GetRandomPosition()
        {
            return transform.TransformPoint(Random.insideUnitCircle.normalized * spawnRadius);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
