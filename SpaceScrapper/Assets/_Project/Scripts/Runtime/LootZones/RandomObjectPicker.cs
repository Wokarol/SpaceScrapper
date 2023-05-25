using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wokarol.SpaceScrapper
{
    public class RandomObjectPicker : MonoBehaviour
    {
        [SerializeField] private bool usePrefabs;
        [SerializeField] private List<GameObject> options = new();

        private InputAction rollAgainDebug;

        private void Awake()
        {
            RollRandomObject();
            rollAgainDebug = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/f10");
            rollAgainDebug.performed += e => RollRandomObject();

            rollAgainDebug.Enable();
        }

        private void OnDestroy()
        {
            rollAgainDebug.Disable();
            rollAgainDebug.Dispose();
        }

        private void RollRandomObject()
        {
            var randomOption = options[Random.Range(0, options.Count)];

            if (usePrefabs)
            {
                Instantiate(randomOption, Vector3.zero, Quaternion.identity, transform);
            }
            else
            {
                for (int i = 0; i < options.Count; i++)
                {
                    var o = options[i];
                    o.SetActive(o == randomOption);
                }
            }
        }
    }
}
