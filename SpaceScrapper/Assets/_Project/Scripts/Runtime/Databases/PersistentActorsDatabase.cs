using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.SpaceScrapper.Saving;

namespace Wokarol.SpaceScrapper.Databases
{
    [CreateAssetMenu(menuName = "Databases/Persistent Actors")]
    public class PersistentActorsDatabase : ScriptableObject
    {
        [SerializeField] private List<PersistentActor> actors = new();

        public IReadOnlyList<PersistentActor> AllActors => actors;

        public PersistentActor GetByKey(string key)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i].Key != key)
                    continue;

                return actors[i];
            }
            return null;
        }
    }
}
