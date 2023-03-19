using UnityEngine;
using Wokarol.GameSystemsLocator;

namespace Wokarol.Common
{
    public class ContextMainCameraBinder : MonoBehaviour
    {
        private void OnEnable()
        {
            GameSystems.Get<SceneContext>().MainCamera = GetComponent<Camera>();
        }

        private void OnDisable()
        {
            GameSystems.Get<SceneContext>().MainCamera = null;
        }
    }
}
