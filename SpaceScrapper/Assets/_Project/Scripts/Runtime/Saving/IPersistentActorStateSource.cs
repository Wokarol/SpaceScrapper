using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Wokarol.SpaceScrapper.Saving
{
    public interface IPersistentActorStateSource
    {
        public void SaveState(PersistentActorStateWriter writer);
        public void LoadState(PersistentActorStateReader reader);
    }

    public struct PersistentActorStateWriter
    {
        private Dictionary<string, object> data;

        public PersistentActorStateWriter(Dictionary<string, object> data)
        {
            this.data = data;
        }

        public void Write<T>(string key, T value) where T : class
        {
            data[key] = value;
        }
    }

    public struct PersistentActorStateReader
    {
        private Dictionary<string, object> data;
        private JsonSerializer serializer;

        public PersistentActorStateReader(Dictionary<string, object> data, JsonSerializer serializer)
        {
            this.data = data;
            this.serializer = serializer;
        }

        public T Read<T>(string key) where T : class
        {
            if (!data.TryGetValue(key, out var obj))
                throw new Exception($"Cannot find the key \"{key}\"");

            if (typeof(T) == typeof(string)) return (T)obj;
            if (typeof(T) == typeof(int)) return (T)obj;
            if (typeof(T) == typeof(float)) return (T)obj;
            if (typeof(T) == typeof(bool)) return (T)obj;

            if (obj is JContainer container)
                return container.ToObject<T>(serializer);

            throw new Exception("Yeah... I have no idea what to do here");
        }
    }
}
