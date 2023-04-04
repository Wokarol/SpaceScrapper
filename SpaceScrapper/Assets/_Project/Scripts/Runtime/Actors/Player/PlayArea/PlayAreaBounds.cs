using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.SpaceScrapper.Actors.PlayBounds
{
    public class PlayAreaBounds : MonoBehaviour
    {
        // TODO: Make bounds system work together with the service locator
        public static List<PlayAreaBounds> AllBounds = new();

        [SerializeField] private PlayAreaShape shape;

        [ShowIf(nameof(shape), PlayAreaShape.Circle)]
        [SerializeField] private float radius = 10;

        private void OnEnable()
        {
            AllBounds.Add(this);
        }

        private void OnDisable()
        {
            AllBounds.Remove(this);
        }

        public bool IsWithinBounds(Vector3 position)
        {
            if (shape == PlayAreaShape.Circle)
            {
                return Vector2.Distance(position, transform.position) < radius;
            }

            return true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (shape == PlayAreaShape.Circle)
            {
                UnityEditor.Handles.color = Color.red;
                UnityEditor.Handles.DrawWireDisc(transform.position, transform.forward, radius);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void ResetStatic()
        {
            AllBounds.Clear();
        }
#endif

        public enum PlayAreaShape
        {
            None,
            Circle,
        }
    }
}
