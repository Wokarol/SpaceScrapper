using UnityEngine;

namespace Wokarol.SpaceScrapper.EyeCandy
{
    public class MoveAroundWithNoise : MonoBehaviour
    {
        [SerializeField] float maxDistance = 20;
        [SerializeField] float noiseSpeed = 5;
        [SerializeField] private float seed = 0;

        private void Update()
        {
            float x = Mathf.PerlinNoise(Time.time * noiseSpeed, 344.671f + seed);
            float y = Mathf.PerlinNoise(Time.time * noiseSpeed, 917.123f + seed);

            transform.localPosition = new Vector3(x, y, 0) * maxDistance;
        }
    }
}
