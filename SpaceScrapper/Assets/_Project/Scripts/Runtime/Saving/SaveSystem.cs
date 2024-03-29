﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Actors;
using Wokarol.SpaceScrapper.Global;
using Wokarol.SpaceScrapper.Saving.DataContainers;

namespace Wokarol.SpaceScrapper.Saving
{
    [AddComponentMenu("Space Scrapper/Systems/Save System")]
    public class SaveSystem : MonoBehaviour
    {
        public const string BeforeWaveSaveslotFile = "before-wave";
        public const string AfterWaveSaveslotFile = "after-wave";

        private List<PersistentSceneController> persistentScenes = new();
        private JsonConverter[] converters = new JsonConverter[]
        {
            new Converters.JsonVector2Converter(),
            new Converters.JsonVector3Converter(),
        };

        private SaveDataContainer saveDataContainer;
        private JsonSerializer serializer;

        private static readonly string saveExtension = ".sav";

        public static string SaveDirectory => Path.Combine(Application.persistentDataPath, "Saves");

        public SaveDataMetadata LastSaveMetadata => (saveDataContainer?.Metadata).GetValueOrDefault();


        private void Awake()
        {
            saveDataContainer = new SaveDataContainer();
            serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings()
            {
                Converters = converters,
                Formatting = Formatting.Indented,
            });
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

        public List<FileNameAndMetadata> GetAllSaveMetdata()
        {
            var directory = SaveDirectory;

            if (!Directory.Exists(directory))
                return new List<FileNameAndMetadata>();

            var metadatas = new List<FileNameAndMetadata>();
            var filesPaths = Directory.GetFiles(directory, $"*.sav");
            foreach (var filePath in filesPaths)
            {
                using var file = File.Open(filePath, FileMode.Open);
                using var textReader = new StreamReader(file);
                using var jsonReader = new JsonTextReader(textReader);

                var minimalSave = serializer.Deserialize<SaveDataContainerMinimal>(jsonReader);

                metadatas.Add(new FileNameAndMetadata()
                {
                    FileName = Path.GetFileNameWithoutExtension(filePath),
                    Metadata = minimalSave.Metadata,
                });
            }
            return metadatas;
        }

        public void SaveGame(string saveName = null)
        {
            saveName ??= DateTime.Now.ToString("yyyy-MM-dd-HH-mm");

            string gameName = GameSystems.Get<GameSettings>().GameName;
            saveDataContainer.Metadata = new SaveDataMetadata()
            {
                SaveName = gameName,
                Date = DateTime.Now,
            };

            foreach (var scene in persistentScenes)
            {
                var sceneContainer = new PersistentSceneDataContainer();

                scene.SaveScene(sceneContainer);

                saveDataContainer.Places[scene.Key] = sceneContainer;
            }

            var player = GameSystems.Get<SceneContext>().Player;
            if (player != null)
                saveDataContainer.Player = Player.Memento.CreateFrom(player);


            var director = GameSystems.Get<GameDirector>();
            if (director != null)
                saveDataContainer.GameState = GameDirector.Memento.CreateFrom(director);

            var (directory, path) = GetDirectoryAndPath(saveName, gameName);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using var file = File.Open(path, FileMode.OpenOrCreate);
            using var textWriter = new StreamWriter(file);


            serializer.Serialize(textWriter, saveDataContainer);

            Debug.Log($"Game saved at \"{path}\"");
        }

        public void LoadGame(string saveName)
        {
            if (saveName == null) throw new Exception("Cannot load a save with no name");

            string gameName = GameSystems.Get<GameSettings>().GameName;

            var (_, path) = GetDirectoryAndPath(saveName, gameName);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"Could not find \"{path}\"");
                return;
            }

            Debug.Log($"Loading game from \"{saveName}\"");


            using var file = File.Open(path, FileMode.Open);
            using var textReader = new StreamReader(file);
            using var jsonReader = new JsonTextReader(textReader);

            saveDataContainer = serializer.Deserialize<SaveDataContainer>(jsonReader);

            GameSystems.Get<GameSettings>().GameName = saveDataContainer.Metadata.SaveName;

            foreach (var scene in persistentScenes)
            {
                if (!saveDataContainer.Places.TryGetValue(scene.Key, out var sceneContainer))
                    return;

                scene.LoadScene(sceneContainer, serializer);
            }

            var player = GameSystems.Get<SceneContext>().Player;
            saveDataContainer.Player.InjectInto(player);

            var director = GameSystems.Get<GameDirector>();
            saveDataContainer.GameState.InjectInto(director);
        }

        private static (string directory, string path) GetDirectoryAndPath(string saveName, string gameName)
        {
            string directory = SaveDirectory;
            string fileName = saveName.Contains('.')
                ? saveName
                : $"{NormalizeGameNameToFileName(gameName)}.{saveName}";

            string path = Path.Combine(directory, $"{fileName}{saveExtension}");
            return (directory, path);
        }

        internal void AddPersistentScene(PersistentSceneController persistentSceneController)
        {
            persistentScenes.Add(persistentSceneController);
        }

        internal void RemovePersistentScene(PersistentSceneController persistentSceneController)
        {
            persistentScenes.Remove(persistentSceneController);
        }

        private static string NormalizeGameNameToFileName(string saveName)
        {
            saveName = Regex.Replace(saveName, @"\s+|_+|-+", "-");
            saveName = Regex.Replace(saveName, @"[^A-Za-z0-9-]", "");
            return saveName.ToLower();
        }

        public struct FileNameAndMetadata
        {
            public string FileName;
            public SaveDataMetadata Metadata;
        }
    }
}
