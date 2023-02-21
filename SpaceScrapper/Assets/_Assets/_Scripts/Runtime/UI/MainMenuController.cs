using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Global;

namespace Wokarol.SpaceScrapper.Menus
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            startButton.onClick.AddListener(StartGame);
            quitButton.onClick.AddListener(QuitGame);
        }

        private void StartGame()
        {
            GameSystems.Get<SceneDirector>().StartNewGame();
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}
