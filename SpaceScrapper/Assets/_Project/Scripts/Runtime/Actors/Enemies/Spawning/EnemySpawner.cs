using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

namespace Wokarol.SpaceScrapper.Actors.Spawning
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy enemyPrefab = null;
        [Space]
        [SerializeField] private List<SpawnPoint> spawnPoints = new();
        [SerializeField] private float intervalBetweenSpawns = 0.4f;

        public async UniTask SpawnWave(int enemiesToSpawn, Action<Enemy> whenSpawned = null, Action<Enemy> whenDied = null)
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                var enemy = Instantiate(enemyPrefab, GetRandomPosition(), Quaternion.identity);
                SceneManager.MoveGameObjectToScene(enemy.gameObject, gameObject.scene);

                if (whenDied != null)
                    enemy.Died += () => whenDied(enemy);

                whenSpawned?.Invoke(enemy);

                await UniTask.Delay(TimeSpan.FromSeconds(intervalBetweenSpawns));
            }
        }

        private Vector3 GetRandomPosition()
        {
            return spawnPoints[Random.Range(0, spawnPoints.Count)].GetRandomPosition();
        }

        public void SpawnEnemies(int enemyCount)
        {
            SpawnWave(enemyCount).Forget();
        }
    }
}
