using UnityEngine;

namespace Wokarol.SpaceScrapper.Pooling
{
    public class DestroyPoolHolderAfterPoolIsSpent : MonoBehaviour
    {
        private void Start()
        {
            int childCount = transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (!child.gameObject.activeSelf)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private void Update()
        {
            if (transform.childCount == 0) Destroy(gameObject);
        }
    }
}
