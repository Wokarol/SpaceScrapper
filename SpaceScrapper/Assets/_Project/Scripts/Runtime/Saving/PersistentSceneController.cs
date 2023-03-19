using System;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Saving.DataContainers;

namespace Wokarol.SpaceScrapper.Saving
{
    [AddComponentMenu("Space Scrapper/Saving/Persistent Scene Cotroller")]
    public class PersistentSceneController : MonoBehaviour
    {
        [SerializeField] private string key = "[null]";
        [SerializeField] private List<PersistentActor> actorsInTheScene = new();

        public string Key => key;

        private void OnEnable()
        {
            GameSystems.Get<SaveSystem>().AddPersistentScene(this);
        }

        private void OnDisable()
        {
            GameSystems.Get<SaveSystem>().RemovePersistentScene(this);
        }

        internal void SaveScene(PersistentSceneDataContainer sceneContainer)
        {
            foreach (PersistentActor actor in actorsInTheScene)
            {
                sceneContainer.Actors.Add(new SavedActorContainer()
                {
                    Key = actor.Key,
                    Data = actor.GetData()
                });

            }
        }
    }
}
