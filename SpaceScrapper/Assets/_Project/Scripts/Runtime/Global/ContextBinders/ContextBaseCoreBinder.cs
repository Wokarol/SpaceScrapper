using UnityEngine;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Actors;

namespace Wokarol.Common
{
    public class ContextBaseCoreBinder : MonoBehaviour
    {
        private void OnEnable()
        {
            GameSystems.Get<SceneContext>().BaseCore = GetComponent<BaseCore>();
        }

        private void OnDisable()
        {
            GameSystems.Get<SceneContext>().BaseCore = null;
        }
    }
}
