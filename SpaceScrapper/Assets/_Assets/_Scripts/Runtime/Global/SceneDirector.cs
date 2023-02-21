using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Wokarol.SpaceScrapper.Global
{
    public class SceneDirector : MonoBehaviour
    {
        [SerializeField, Scene] private List<string> gameplayScenes = new();
        [SerializeField, Scene] private string hubScene = "";
        [SerializeField, Scene] private string menuScene = "";

        public void RemoveAllAndLoadGameplayScenes()
        {
            for (int i = 0; i < gameplayScenes.Count; i++)
            {
                SceneManager.LoadScene(gameplayScenes[i], i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
            }
        }

        public void LoadIntoHub()
        {
            SceneManager.LoadScene(hubScene, LoadSceneMode.Additive);
        }

        public void StartNewGame()
        {
            RemoveAllAndLoadGameplayScenes();
            LoadIntoHub();
        }

        public void OpenMainMenu()
        {
            SceneManager.LoadScene(menuScene, LoadSceneMode.Single);
        }
    }
}
