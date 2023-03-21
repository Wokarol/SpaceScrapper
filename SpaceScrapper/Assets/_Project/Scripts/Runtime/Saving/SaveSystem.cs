using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        private SaveDataContainer saveDataContainer;

        private void Awake()
        {
            saveDataContainer = new SaveDataContainer();
        }

        private void Update()
        {
            if (Keyboard.current.f5Key.wasPressedThisFrame)
            {
                SaveGame("autosave");
            }


            if (Keyboard.current.f9Key.wasPressedThisFrame)
            {
                LoadGame("autosave");
            }

        }

        public void SaveGame(string saveName)
        {
            foreach (var scene in persistentScenes)
            {
                var sceneContainer = new PersistentSceneDataContainer();

                scene.SaveScene(sceneContainer);

                saveDataContainer.Places[scene.Key] = sceneContainer;
            }

            string json = JsonConvert.SerializeObject(saveDataContainer, Formatting.Indented, converters);
            Debug.Log(json);
            Debug.Log($"Game saved at \"{saveName}\"");
        }

        public void LoadGame(string saveName)
        {
            Debug.Log($"Loading game from \"{saveName}\"");
            throw new NotImplementedException();
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
