using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Wokarol.SpaceScrapper.Saving.DataContainers;

namespace Wokarol.SpaceScrapper.Saving
{
    [AddComponentMenu("Space Scrapper/Systems/Save System")]
    public class SaveSystem : MonoBehaviour
    {
        private List<PersistentSceneController> persistentScenes = new();
        private JsonConverter[] converters = new JsonConverter[]
        {
            new Converters.JsonVector2Converter(),
            new Converters.JsonVector3Converter(),
        };

        private void Update()
        {
            if (Keyboard.current.f5Key.wasPressedThisFrame)
            {
                SaveGame();
            }

        }

        private void SaveGame()
        {
            var saveDataContainer = new SaveDataContainer();
            foreach (var scene in persistentScenes)
            {
                var sceneContainer = new PersistentSceneDataContainer();

                scene.SaveScene(sceneContainer);

                saveDataContainer.Places.Add(scene.Key, sceneContainer);
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(saveDataContainer, Formatting.Indented, converters);
            Debug.Log(json);
        }

        internal void AddPersistentScene(PersistentSceneController persistentSceneController)
        {
            persistentScenes.Add(persistentSceneController);
        }

        internal void RemovePersistentScene(PersistentSceneController persistentSceneController)
        {
            persistentScenes.Remove(persistentSceneController);
        }
    }
}
