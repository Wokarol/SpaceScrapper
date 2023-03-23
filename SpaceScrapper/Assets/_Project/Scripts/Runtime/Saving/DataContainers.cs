using Newtonsoft.Json;
using System.Collections.Generic;

namespace Wokarol.SpaceScrapper.Saving.DataContainers
{
    public class SaveDataContainer
    {
        [JsonProperty("places")]
        public Dictionary<string, PersistentSceneDataContainer> Places { get; private set; } = new();
    }

    public class PersistentSceneDataContainer
    {
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
}