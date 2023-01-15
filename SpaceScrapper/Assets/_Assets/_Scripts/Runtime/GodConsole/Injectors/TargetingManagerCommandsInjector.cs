using System.Linq;
using UnityEngine;
using Wokarol.GodConsole;
using Wokarol.SpaceScrapper.Combat;

namespace Wokarol.SpaceScrapper.GodConsole.Injectors
{
    public class TargetingManagerCommandsInjector : MonoBehaviour, IInjector
    {
        public void Inject(Wokarol.GodConsole.GodConsole.CommandBuilder b)
        {
            b.Group("targeting")
                .Add("list", (TargetingManager m) =>
                {
                    foreach (var faction in m.AllActors.GroupBy(a => a.Faction))
                    {
                        Debug.Log($"{faction.Key.name}: [{faction.Count()}] {string.Join(", ", m.AllActors.OrderBy(a => a.Faction).Select(a => $"{a.name}"))}");
                    }
                });
        }
    }
}
