using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Global;

namespace Wokarol.SpaceScrapper.UI
{

    public class GameplayGUI : MonoBehaviour
    {
        [SerializeField] private PlayerView playerView = null;
        [SerializeField] private DirectorView directorView = null;
        [SerializeField] private BaseCoreView coreView = null;

        private void LateUpdate()
        {
            BindTheUIUsingContext();
        }

        private void BindTheUIUsingContext()
        {
            if (!playerView.IsBound)
            {
                // Here, and event could be nice to not repeat this call too often
                var player = GameSystems.Get<SceneContext>().Player;
                if (player != null) playerView.BindTo(player);
            }

            if (!directorView.IsBound)
            {
                var director = GameSystems.Get<GameDirector>();
                if (director != null) directorView.BindTo(director);
            }

            if (!coreView.IsBound)
            {
                var core = GameSystems.Get<SceneContext>().BaseCore;
                if (core != null) coreView.BindTo(core);
            }
        }
    }
}
