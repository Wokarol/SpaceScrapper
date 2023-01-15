using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.SpaceScrapper.Actors;

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
        [Space]
        [SerializeField] private bool spawnOnStart;

        int aliveEnemies = 0;
        bool waveInProgress = false;
        bool awaitingWave = false;

        float waveCountdown;

        private void Awake()
        {
            if (spawnOnStart)
            {
                waveCountdown = timeBetweenWaves;
                awaitingWave = true;
            }
        }

        private void Update()
        {
            if (!waveInProgress && awaitingWave)
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

        public void SpawnEnemies(int enemyCount)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                Instantiate(enemyPrefab, GetRandomPosition(), Quaternion.identity, transform);
            }
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
