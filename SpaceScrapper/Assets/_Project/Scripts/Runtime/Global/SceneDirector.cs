using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Wokarol.SpaceScrapper.Global
{
    [DefaultExecutionOrder(-250)]
    public class SceneDirector : MonoBehaviour
    {
        [SerializeField, Scene] private List<string> gameplayScenes = new();
        [SerializeField, Scene] private string hubScene = "";
        [SerializeField, Scene] private string menuScene = "";

        private HashSet<object> awaitedObjects = new();
        private bool isLoadingScenes = new();

        public bool AreScenesReady => awaitedObjects.Count == 0 && !isLoadingScenes;
        public UniTask WaitUntilScenesAreReady => UniTask.WaitUntil(() => AreScenesReady);

        // Note: At the moment of calling this method, scenes are (probably) not yet loaded
        private void Awake()
        {
            isLoadingScenes = true;
            var loadedScenes = GetAllLoadedScenes();

            if (IsInMenu(loadedScenes))
            {
                Debug.Log("<color=cyan>Scene Director:</color> Started in the menu");
            }

            if (IsInHub(loadedScenes))
            {
                Debug.Log("<color=cyan>Scene Director:</color> Started in the Hub");
            }

            if (IsUsingGameplayScenes(loadedScenes, out bool allGameplayScenesAreLoaded))
            {
                if (allGameplayScenesAreLoaded)
                {
                    Debug.Log("<color=cyan>Scene Director:</color> Started with the gameplay scenes");
                }
                else
                {
                    Debug.Log("<color=cyan>Scene Director:</color> Started with the gameplay scenes, not all scenes are loaded");
                }
            }


            bool IsInMenu(HashSet<string> loadedScenes)
            {
                return loadedScenes.Contains(menuScene);
            }

            bool IsInHub(HashSet<string> loadedScenes)
            {
                return loadedScenes.Contains(hubScene);
            }

            bool IsUsingGameplayScenes(HashSet<string> loadedScenes, out bool allGameplayScenesAreLoaded)
            {
                bool anyGameplaySceneIsLoaded = false;
                allGameplayScenesAreLoaded = true;

                for (int i = 0; i < gameplayScenes.Count; i++)
                {
                    if (loadedScenes.Contains(gameplayScenes[i]))
                        anyGameplaySceneIsLoaded = true;
                    else
                        allGameplayScenesAreLoaded = false;
                }

                return anyGameplaySceneIsLoaded;
            }
        }

        private void Start()
        {
            isLoadingScenes = false;    
        }

        private static HashSet<string> GetAllLoadedScenes()
        {
            HashSet<string> loadedScenes = new();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            return loadedScenes;
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

        public void AwaitObjectToBeReady(object obj)
        {
            awaitedObjects.Add(obj);
        }

        public void MarkObjectAsReady(object obj)
        {
            awaitedObjects.Remove(obj);
        }
    }
}
