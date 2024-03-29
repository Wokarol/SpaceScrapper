using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.SpaceScrapper.Saving
{
    [AddComponentMenu("Space Scrapper/Saving/Persistent Actor")]
    public class PersistentActor : MonoBehaviour
    {
        [SerializeField] private string key = "[null]";
        [Space]
        [SerializeField] private bool savePlacement = true;

        public string Key => key;

        private Dictionary<string, object> cachedDataCollection = new();
        private IPersistentActorStateSource[] stateSources = null;

        private void OnValidate()
        {
            if (key == "[null]")
                key = gameObject.name.ToLower().Replace(' ', '-');
        }

        private void Awake()
        {
            stateSources = GetComponents<IPersistentActorStateSource>();
            PersistentSceneController.RegisterActor(this);
        }

        private void OnDestroy()
        {
            PersistentSceneController.UnregisterActor(this);
        }

        [ContextMenu("Reset key")]
        public void ResetKey() => key = "[null]";

        [ContextMenu("Generate random GUID")]
        public void GenerateGUID() => key = System.Guid.NewGuid().ToString();

        public Dictionary<string, object> GetData()
        {
            cachedDataCollection.Clear();

            var writer = new PersistentActorStateWriter(cachedDataCollection);

            if (savePlacement)
            {
                writer.Write("actor-placement", new ActorPlacementMemento()
                {
                    pos = transform.position,
                    rot = transform.eulerAngles.z
                });
            }

            foreach (var source in stateSources)
            {
                source.SaveState(writer);
            }

            return cachedDataCollection;
        }

        internal void LoadData(Dictionary<string, object> data, Newtonsoft.Json.JsonSerializer serializer)
        {
            cachedDataCollection = data;
            var reader = new PersistentActorStateReader(cachedDataCollection, serializer);

            if (savePlacement)
            {
                var placement = reader.Read<ActorPlacementMemento>("actor-placement");

                transform.position = placement.pos;

                var angles = transform.eulerAngles;
                angles.z = placement.rot;
                transform.eulerAngles = angles;
            }

            foreach (var source in stateSources)
            {
                source.LoadState(reader);
            }
        }

        public class ActorPlacementMemento
        {
            public Vector2 pos;
            public float rot;
        }
    }
}
