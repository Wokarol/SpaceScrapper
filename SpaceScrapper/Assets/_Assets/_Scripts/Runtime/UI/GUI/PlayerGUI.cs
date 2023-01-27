using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Actors;

namespace Wokarol.SpaceScrapper.UI
{
    public class PlayerGUI : MonoBehaviour
    {
        [SerializeField] private UIValueBar healthBar;

        private Player player;

        private void Start()
        {
            player = GameSystems.Get<SceneContext>().Player;
        }

        private void LateUpdate()
        {
            healthBar.Value = player.Health / (float)player.MaxHealth;
        }
    }
}
