using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;

namespace Wokarol.SpaceScrapper.Combat
{
    public class TargetPointerController : MonoBehaviour
    {
        [SerializeField] private CombatActor self = null;
        [SerializeField] private SpriteRenderer pointerTemplate = null;

        private List<SpriteRenderer> pointers = new();

        private void Start()
        {
            pointerTemplate.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            for (int i = 0; i < pointers.Count; i++)
            {
                pointers[i].gameObject.SetActive(false);
            }

            Vector2 origin = pointerTemplate.transform.position;
            int pointerI = 0;
            for (int i = 0; i < GameSystems.Get<TargetingManager>().AllActors.Count; i++)
            {
                var actor = GameSystems.Get<TargetingManager>().AllActors[i];
                if (actor.Faction == self.Faction) continue;

                var direction = (Vector2)actor.transform.position - origin;
                var pointer = GetOrCreatePointer(pointerI);

                pointer.transform.up = direction;
                pointer.gameObject.SetActive(true);

                pointerI++;
            }
        }

        private SpriteRenderer GetOrCreatePointer(int i)
        {
            if (i < pointers.Count) return pointers[i];
            else
            {
                Debug.Log($"Adding pointer at {i}");
                var pointer = Instantiate(pointerTemplate, pointerTemplate.transform.parent);
                pointers.Add(pointer);
                return pointer;
            }
        }
    }
}
