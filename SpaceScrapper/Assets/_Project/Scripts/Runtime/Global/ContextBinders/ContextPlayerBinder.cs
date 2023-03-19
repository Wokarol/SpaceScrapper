using UnityEngine;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Actors;

namespace Wokarol.Common
{

    public class ContextPlayerBinder : MonoBehaviour
    {
        private void OnEnable()
        {
            GameSystems.Get<SceneContext>().Player = GetComponent<Player>();
        }

        private void OnDisable()
        {
            GameSystems.Get<SceneContext>().Player = null;
        }
    }
}
