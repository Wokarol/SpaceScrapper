using UnityEngine;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Player;

namespace Wokarol.Common
{
    public class ContextPlayerBinder : MonoBehaviour
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
            GameSystems.Get<SceneContext>().Player = GetComponent<Player>();
        }

        private void OnDisable()
        {
            if (!started) return;
            GameSystems.Get<SceneContext>().Player = null;
        }
    }
}
