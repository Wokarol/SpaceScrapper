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

        private void LateUpdate()
        {
            if (player == null)
                // Here, and event could be nice to not repeat this call too often
                player = GameSystems.Get<SceneContext>().Player;

            if (player == null)
            {
                healthBar.gameObject.SetActive(false);
            }
            else
            {
                healthBar.gameObject.SetActive(true);
                healthBar.Value = player.Health / (float)player.MaxHealth;
            }
        }
    }
}
