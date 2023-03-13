using UnityEngine;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper;
using Wokarol.SpaceScrapper.Actors.Spawning;

namespace Wokarol.Common
{
    public class WaveEnemySpawnerContextBinder : MonoBehaviour
    {
        private void OnEnable()
        {
            GameSystems.Get<SceneContext>().WaveEnemySpawner = GetComponent<EnemySpawner>();
        }

        private void OnDisable()
        {
            GameSystems.Get<SceneContext>().WaveEnemySpawner = null;
        }
    }
}
