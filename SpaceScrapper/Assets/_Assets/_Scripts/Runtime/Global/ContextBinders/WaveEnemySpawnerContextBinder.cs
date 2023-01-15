using UnityEngine;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper;

namespace Wokarol.Common
{
    public class WaveEnemySpawnerContextBinder : MonoBehaviour
    {
        bool started;
        private void Start()
        {
            started = true;
            OnEnable();
        }

        private void OnEnable()
        {
            if (!started) return;
            GameSystems.Get<SceneContext>().WaveEnemySpawner = GetComponent<EnemySpawner>();
        }

        private void OnDisable()
        {
            if (!started) return;
            GameSystems.Get<SceneContext>().WaveEnemySpawner = null;
        }
    }
}
