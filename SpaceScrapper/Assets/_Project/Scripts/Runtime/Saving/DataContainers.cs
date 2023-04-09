using Newtonsoft.Json;
using System.Collections.Generic;
using Wokarol.SpaceScrapper.Actors;
using Wokarol.SpaceScrapper.Global;

namespace Wokarol.SpaceScrapper.Saving.DataContainers
{
    public class SaveDataContainerMinimal
    {
        [JsonProperty("metadata")]
        public SaveDataMetadata Metadata { get; set; }
    }

    public class SaveDataContainer : SaveDataContainerMinimal
    {
        [JsonProperty("player")]
        public Player.Memento Player { get; set; }

        [JsonProperty("game-state")]
        public GameDirector.Memento GameState { get; set; }

        [JsonProperty("places")]
        public Dictionary<string, PersistentSceneDataContainer> Places { get; private set; } = new();
    }

    public class PersistentSceneDataContainer
    {

        [JsonProperty("data")]
        public Dictionary<string, object> Data { get; set; }

        [JsonProperty("actors")]
        public List<SavedActorContainer> Actors { get; private set; } = new();
    }

    public struct SavedActorContainer
    {
        [JsonProperty("key")]
        public string Key;
        [JsonProperty("data")]
        public Dictionary<string, object> Data;
    }

    public struct SaveDataMetadata
    {
        [JsonProperty("save-name")]
        public string SaveName { get; set; }
    }
}