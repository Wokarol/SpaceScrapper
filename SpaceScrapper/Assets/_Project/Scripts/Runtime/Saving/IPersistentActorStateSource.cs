using System.Collections.Generic;
using System;

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

        public PersistentActorStateReader(Dictionary<string, object> data)
        {
            this.data = data;
        }

        public T Read<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
