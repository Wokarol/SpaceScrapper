using UnityEngine;
using Wokarol.Common;
using Wokarol.GodConsole;

namespace Wokarol.SpaceScrapper.GodConsole.Injectors
{
    public class EnemySpawnerCommandsInjector : MonoBehaviour, IInjector
    {
        public void Inject(Wokarol.GodConsole.GodConsole.CommandBuilder b)
        {
            b.Add("spawn enemies", (int count, SceneContext ctx) => ctx.WaveEnemySpawner.SpawnEnemies(count));
            b.Add("spawn enemies", (SceneContext ctx) => ctx.WaveEnemySpawner.SpawnEnemies(1));
        }
    }
}
