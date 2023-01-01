using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Wokarol.SpaceScrapper.Menus
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;
        [Space]
        [SerializeField, Scene] private List<string> scenesToLoadOnStart = new();

        private void Start()
        {
            startButton.onClick.AddListener(StartGame);
            quitButton.onClick.AddListener(QuitGame);
        }

        private void StartGame()
        {
            for (int i = 0; i < scenesToLoadOnStart.Count; i++)
            {
                SceneManager.LoadSceneAsync(scenesToLoadOnStart[i], i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
            }
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}
