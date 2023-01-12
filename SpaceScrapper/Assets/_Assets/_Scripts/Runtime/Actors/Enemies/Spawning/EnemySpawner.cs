using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.SpaceScrapper
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy enemyPrefab = null;
        [Space]
        [SerializeField] private int enemiesPerWave = 5;
        [SerializeField] private float timeBetweenWaves = 3;
        [Space]
        [SerializeField] private float spawnRadius = 16;

        int aliveEnemies = 0;
        bool waveInProgress = false;

        float waveCountdown;

        private void Awake()
        {
            waveCountdown = timeBetweenWaves;
        }

        private void Update()
        {
            if (!waveInProgress)
            {
                waveCountdown -= Time.deltaTime;

                if (waveCountdown < 0)
                {
                    SpawnWave();
                }
            }
        }

        private void SpawnWave()
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                var enemy = Instantiate(enemyPrefab, GetRandomPosition(), Quaternion.identity, transform);
                enemy.Died += EnemyDied;

                aliveEnemies++;
            }
            waveInProgress = true;
        }

        private Vector2 GetRandomPosition()
        {
            return Random.insideUnitCircle.normalized * spawnRadius;
        }

        private void EnemyDied()
        {
            aliveEnemies--;

            if (aliveEnemies <= 0)
            {
                waveCountdown = timeBetweenWaves;
                waveInProgress = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
