using System;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.SpaceScrapper;
using Wokarol.SpaceScrapper.Actors;

namespace Wokarol.Common
{
    public class SceneContext : MonoBehaviour
    {
        [Header("[Filled in code]")]
        public Camera MainCamera;
        public Player Player;
        public EnemySpawner WaveEnemySpawner;
        public BaseCore BaseCore;

        private readonly List<PlayerSpawnPosition> spawnPoints = new();
        public IReadOnlyList<PlayerSpawnPosition> SpawnPoints => spawnPoints;


        internal void AddSpawnPoint(PlayerSpawnPosition playerSpawnPosition)
        {
            spawnPoints.Add(playerSpawnPosition);
        }

        internal void RemoveSpawnPoint(PlayerSpawnPosition playerSpawnPosition)
        {
            spawnPoints.Remove(playerSpawnPosition);
        }
    }
}
