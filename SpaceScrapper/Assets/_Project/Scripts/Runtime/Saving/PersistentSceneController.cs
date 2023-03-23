using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Saving.DataContainers;

namespace Wokarol.SpaceScrapper.Saving
{
    [DefaultExecutionOrder(-50)]
    [AddComponentMenu("Space Scrapper/Saving/Persistent Scene Cotroller")]
    public class PersistentSceneController : MonoBehaviour
    {
        [SerializeField] private string key = "[null]";
        [SerializeField] private List<PersistentActor> actorsInTheScene = new();
        [Space]
        [SerializeField] private List<PersistentActor> prefabsToSpawn = new();

        public string Key => key;

        private static List<PersistentSceneController> persistentSceneControllers = new();

        private void OnEnable()
        {
            GameSystems.Get<SaveSystem>().AddPersistentScene(this);
            persistentSceneControllers.Add(this);
        }

        private void OnDisable()
        {
            GameSystems.Get<SaveSystem>().RemovePersistentScene(this);
            persistentSceneControllers.Remove(this);
        }

        private void AddActor(PersistentActor actor)
        {
            if (actorsInTheScene.Contains(actor))
                return;

            actorsInTheScene.Add(actor);
        }

        private void RemoveActor(PersistentActor actor)
        {
            actorsInTheScene.Remove(actor);
        }

        internal void SaveScene(PersistentSceneDataContainer sceneContainer)
        {
            foreach (PersistentActor actor in actorsInTheScene)
            {
                sceneContainer.Actors.Add(new SavedActorContainer()
                {
                    Key = actor.Key,
                    Data = actor.GetData()
                });

            }
        }

        internal void LoadScene(PersistentSceneDataContainer sceneContainer, Newtonsoft.Json.JsonSerializer serializer)
        {
            List<PersistentActor> sceneActorsToUse = actorsInTheScene;
            actorsInTheScene = new();

            foreach (var savedActor in sceneContainer.Actors)
            {
                var key = savedActor.Key;
                PersistentActor foundActor = null;

                for (int i = 0; i < sceneActorsToUse.Count; i++)
                {
                    if (sceneActorsToUse[i].Key != key)
                        continue;
                    
                    foundActor = sceneActorsToUse[i];
                    sceneActorsToUse.RemoveAt(i);
                    break;
                }

                if (foundActor == null)
                {
                    foundActor = CreateActorFromPrefab(key);
                }

                foundActor.LoadData(savedActor.Data, serializer);
                actorsInTheScene.Add(foundActor);
            }

            foreach (var unusedActor in sceneActorsToUse)
            {
                Destroy(unusedActor.gameObject);
            }
        }

        private PersistentActor CreateActorFromPrefab(string key)
        {

            for (int i = 0; i < prefabsToSpawn.Count; i++)
            {
                if (prefabsToSpawn[i].Key != key)
                    continue;

                var foundActor = prefabsToSpawn[i];

                // This is so the new actor is spawned in the correct scene, that way it gets assigned properly in Awake
                SceneManager.SetActiveScene(gameObject.scene);
                return Instantiate(foundActor);
            }

            return null;
        }

        public static void RegisterActor(PersistentActor actor)
        {
            Scene scene = actor.gameObject.scene;
            foreach (var controller in persistentSceneControllers)
            {
                if (controller.gameObject.scene != scene)
                    continue;

                controller.AddActor(actor);
                return;
            }

            Debug.LogWarning($"There was no controller that handle the appearance of actor \"{actor.name}\" in scene \"{scene.name}\"");
        }


        public static void UnregisterActor(PersistentActor actor)
        {
            Scene scene = actor.gameObject.scene;
            foreach (var controller in persistentSceneControllers)
            {
                if (controller.gameObject.scene != scene)
                    continue;

                controller.RemoveActor(actor);
                return;
            }
        }
    }
}
