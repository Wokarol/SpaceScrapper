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

        private void Start()
        {
            HashSet<string> loadedScenes = new();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            if (loadedScenes.Contains(menuScene))
            {
                Debug.Log("<color=cyan>Scene Director:</color> Started in the menu");
            }

            bool anyGameplaySceneIsLoaded = false;
            bool allScenesAreLoaded = true;

            for (int i = 0; i < gameplayScenes.Count; i++)
            {
                if (loadedScenes.Contains(gameplayScenes[i]))
                    anyGameplaySceneIsLoaded = true;
                else
                    allScenesAreLoaded = false;
            }

            if (anyGameplaySceneIsLoaded && !allScenesAreLoaded)
            {
                if (allScenesAreLoaded)
                {
                    Debug.Log("<color=cyan>Scene Director:</color> Started with the gameplay scenes");
                }
                else
                {
                    Debug.Log("<color=cyan>Scene Director:</color> Started with the gameplay scenes, not all scenes are loaded");
                }
            }

            if (loadedScenes.Contains(hubScene))
            {
                Debug.Log("<color=cyan>Scene Director:</color> Started in the Hub");
            }
        }

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
