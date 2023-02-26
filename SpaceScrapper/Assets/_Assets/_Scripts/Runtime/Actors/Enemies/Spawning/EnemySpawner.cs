using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.SpaceScrapper.Actors;

using Random = UnityEngine.Random;

namespace Wokarol.SpaceScrapper
{

    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy enemyPrefab = null;
        [Space]
        [SerializeField] private List<SpawnPoint> spawnPoints = new();
        [SerializeField] private float intervalBetweenSpawns = 0.4f;

        int aliveEnemies = 0;

        public async UniTask SpawnWave(int enemiesToSpawn)
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                var enemy = Instantiate(enemyPrefab, GetRandomPosition(), Quaternion.identity, transform);
                enemy.Died += EnemyDied;

                aliveEnemies++;

                await UniTask.Delay(TimeSpan.FromSeconds(intervalBetweenSpawns));
            }
        }

        private void EnemyDied()
        {
            aliveEnemies--;
        }

        private Vector3 GetRandomPosition()
        {
            return spawnPoints[Random.Range(0, spawnPoints.Count)].GetRandomPosition();
        }

        public void SpawnEnemies(int enemyCount)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                Instantiate(enemyPrefab, GetRandomPosition(), Quaternion.identity, transform);
            }
        }
    }
}
