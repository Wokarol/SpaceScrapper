using UnityEngine;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper;

namespace Wokarol.Common
{
    public class ContextBaseCoreBinder : MonoBehaviour
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
            GameSystems.Get<SceneContext>().BaseCore = GetComponent<BaseCore>();
        }

        private void OnDisable()
        {
            if (!started) return;
            GameSystems.Get<SceneContext>().BaseCore = null;
        }
    }
}
