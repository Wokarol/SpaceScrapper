using UnityEngine;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Global;
using Wokarol.SpaceScrapper.UI.Views;

namespace Wokarol.SpaceScrapper.UI
{

    public class GameplayGUI : MonoBehaviour
    {
        [SerializeField] private PlayerView playerView = null;
        [SerializeField] private DirectorView directorView = null;
        [SerializeField] private BaseCoreView coreView = null;

        private void Start()
        {
            directorView.GameOverShown += () =>
            {
                playerView.UnbindAndHide();
                coreView.UnbindAndHide();
            };
        }

        private void LateUpdate()
        {
            if (!directorView.IsGameOverShown)
                BindTheUIUsingContext();
        }

        private void BindTheUIUsingContext()
        {
            if (!playerView.IsBound)
            {
                // Here, and event could be nice to not repeat this call too often
                var player = GameSystems.Get<SceneContext>().Player;
                if (player != null) playerView.BindAndShow(player);
            }

            if (!directorView.IsBound)
            {
                var director = GameSystems.Get<GameDirector>();
                if (director != null) directorView.BindAndShow(director);
            }

            if (!coreView.IsBound)
            {
                var core = GameSystems.Get<SceneContext>().BaseCore;
                if (core != null) coreView.BindAndShow(core);
            }
        }
    }
}
