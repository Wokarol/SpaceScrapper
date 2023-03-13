using UnityEngine;
using Wokarol.GameSystemsLocator;
using Wokarol.GodConsole;

namespace Wokarol.Common
{
    public class GameSystemLocatorGodConsoleServiceProvider : MonoBehaviour, IServiceProvider
    {
        public object Get(System.Type type)
        {
            return GameSystems.Get(type);
        }
    }
}
