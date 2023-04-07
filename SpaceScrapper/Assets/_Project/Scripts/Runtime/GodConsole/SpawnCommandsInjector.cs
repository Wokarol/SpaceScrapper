using Cysharp.Threading.Tasks;
using UnityEngine;
using Wokarol.Common;
using Wokarol.GodConsole;
using Wokarol.SpaceScrapper.Databases;
using System.Linq;

namespace Wokarol.SpaceScrapper.GodConsole.Injectors
{
    public class SpawnCommandsInjector : MonoBehaviour, IInjector
    {
        [SerializeField] private PersistentActorsDatabase database;
        [SerializeField] private float spawnInFrontOfPlayerDistance = 2;

        public void Inject(Wokarol.GodConsole.GodConsole.CommandBuilder b)
        {
            b.Group("spawn")
                .Add("actor", (string key, SceneContext ctx) =>
                {
                    var prefab = database.GetByKey(key);
                    if (prefab == null)
                    {
                        Debug.LogError("Could not find a prefab for given key");
                        return;
                    }

                    var player = ctx.Player;
                    var spawnPosition = player != null
                        ? player.transform.TransformPoint(Vector3.up * spawnInFrontOfPlayerDistance)
                        : Vector3.zero;

                    Instantiate(prefab, spawnPosition, Quaternion.identity);
                })
                .Add("actor", () =>
                {
                    Debug.Log($"Available actors: {string.Join(", ", database.AllActors.Select(a => a.Key))}");
                });
        }
    }
}
