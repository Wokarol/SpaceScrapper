using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Wokarol.SpaceScrapper.Global
{

    [DefaultExecutionOrder(-250)]
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField, Scene] private List<string> gameplayScenes = new();
        [SerializeField, Scene] private List<string> lootZoneScenes = new();
        [SerializeField, Scene] private string hubScene = "";
        [SerializeField, Scene] private string menuScene = "";

        private HashSet<object> awaitedObjects = new();
        private bool isLoadingScenes = new();

        private Scene loadedMap;

        public bool AreScenesReady => awaitedObjects.Count == 0 && !isLoadingScenes;

        // Note: At the moment of calling this method, scenes are (probably) not yet loaded
        private void Awake()
        {
            isLoadingScenes = true;
            var loadedScenes = GetAllLoadedScenes();

            if (IsInMenu(out var menu))
            {
                Debug.Log("<color=cyan>Scene Director:</color> Started in the menu");
            }

            if (IsInHub(out var hub))
            {
                loadedMap = hub;
                Debug.Log("<color=cyan>Scene Director:</color> Started in the Hub");
            }

            if (IsInLootZone(out var lootZone))
            {
                loadedMap = lootZone;
                Debug.Log("<color=cyan>Scene Director:</color> Started in the Loot Zone");
            }

            if (IsUsingGameplayScenes(out bool allGameplayScenesAreLoaded))
            {
                if (allGameplayScenesAreLoaded)
                {
                    Debug.Log("<color=cyan>Scene Loader:</color> Started with the gameplay scenes");
                }
                else
                {
                    Debug.Log("<color=cyan>Scene Loader:</color> Started with the gameplay scenes, not all scenes are loaded");
                }
            }


            bool IsInMenu(out Scene scene)
            {
                scene = loadedScenes.FirstOrDefault(s => s.name == menuScene);
                return scene.IsValid();
            }

            bool IsInHub(out Scene scene)
            {
                scene = loadedScenes.FirstOrDefault(s => s.name == hubScene);
                return scene.IsValid();
            }

            bool IsInLootZone(out Scene scene)
            {
                scene = loadedScenes.FirstOrDefault(s => lootZoneScenes.Contains(s.name));
                return scene.IsValid();
            }

            bool IsUsingGameplayScenes(out bool allGameplayScenesAreLoaded)
            {
                bool anyGameplaySceneIsLoaded = false;
                allGameplayScenesAreLoaded = true;

                for (int i = 0; i < gameplayScenes.Count; i++)
                {
                    if (loadedScenes.Any(s => s.name == gameplayScenes[i]))
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

        public void RemoveAllAndLoadGameplayScenes()
        {
            for (int i = 0; i < gameplayScenes.Count; i++)
            {
                SceneManager.LoadScene(gameplayScenes[i], i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
            }
        }

        public void LoadIntoHub()
        {
            UnloadCurrentMapIfOneIsLoaded();

            SceneManager.LoadScene(hubScene, LoadSceneMode.Additive);
            loadedMap = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        }

        public void LoadLootZone()
        {
            UnloadCurrentMapIfOneIsLoaded();

            var randomLootZone = lootZoneScenes[Random.Range(0, lootZoneScenes.Count)];
            SceneManager.LoadScene(randomLootZone, LoadSceneMode.Additive);
            loadedMap = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        }

        public void StartGame()
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



        private static HashSet<Scene> GetAllLoadedScenes()
        {
            HashSet<Scene> loadedScenes = new();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i));
            }

            return loadedScenes;
        }

        private void UnloadCurrentMapIfOneIsLoaded()
        {
            if (loadedMap.IsValid())
                SceneManager.UnloadSceneAsync(loadedMap);
        }
    }
}
