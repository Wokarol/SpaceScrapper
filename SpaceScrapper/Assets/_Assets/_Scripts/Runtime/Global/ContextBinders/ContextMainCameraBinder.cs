using UnityEngine;
using Wokarol.GameSystemsLocator;

namespace Wokarol.Common
{
    public class ContextMainCameraBinder : MonoBehaviour
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
            GameSystems.Get<SceneContext>().MainCamera = GetComponent<Camera>();
        }

        private void OnDisable()
        {
            if (!started) return;
            GameSystems.Get<SceneContext>().MainCamera = null;
        }
    }
}
