using System;
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

        private void OnValidate()
        {
            if (key == "[null]")
                key = gameObject.name.ToLower().Replace(' ', '-');
        }

        [ContextMenu("Reset key")]
        public void ResetKey() => key = "[null]";

        [ContextMenu("Generate random GUID")]
        public void GenerateGUID() => key = System.Guid.NewGuid().ToString();

        public object GetData()
        {
            return new PersistentActorMemento()
            {
                Pos = transform.position,
                Rot = transform.eulerAngles.z
            };
        }

        public struct PersistentActorMemento
        {
            public Vector2 Pos;
            public float Rot;
        }
    }
}
