using UnityEngine;

namespace Wokarol.SpaceScrapper.EyeCandy
{
    public class ScaleWithNoise : MonoBehaviour
    {
        [SerializeField] private Vector3 minScale = Vector3.one;
        [SerializeField] private Vector3 maxScale = new(2, 1, 1);
        [Space]
        [SerializeField] private float noiseSpeed = 1;
        [SerializeField] private float seed = 0;

        private void Update()
        {
            float t = Mathf.PerlinNoise(Time.time * noiseSpeed, seed);

            transform.localScale = Vector3.Lerp(maxScale, minScale, t);
        }
    }
}
