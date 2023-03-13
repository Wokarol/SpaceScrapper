using UnityEngine;

namespace Wokarol.SpaceScrapper.Weaponry
{
    public class PushableByHits : MonoBehaviour, IHittable
    {
        private Rigidbody2D body;

        private void Awake()
        {
            if (!TryGetComponent(out body))
            {
                Debug.LogError("The object does not have a rigidbody", this);
            }
        }

        public void Hit(Vector2 force, Vector2 normal, Vector2 point, int damage)
        {
            if (body != null)
            {
                body.AddForceAtPosition(force, point);
            }
        }
    }
}
