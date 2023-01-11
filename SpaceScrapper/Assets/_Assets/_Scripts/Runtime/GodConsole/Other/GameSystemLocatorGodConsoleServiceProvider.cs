using UnityEngine;
using Wokarol.GameSystemsLocator;
using Wokarol.GodConsole;

namespace Wokarol.Common
{
    public class GameSystemLocatorGodConsoleServiceProvider : MonoBehaviour, IServiceProvider
    {
        public object Get(System.Type type)
        {
            var baseMethod = typeof(GameSystems).GetMethod(nameof(GameSystems.Get));
            var genericMethod = baseMethod.MakeGenericMethod(type);

            return genericMethod.Invoke(null, null);
        }
    }
}
