using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.SpaceScrapper.Actors.Common
{
    public class MultiDirectionalEngineVisualController : MonoBehaviour
    {
        [SerializeField] private List<Transform> thrusters = new();
        [SerializeField] private float thrusterNoise = 0.1f;
        [SerializeField] private float thrusterNoiseSpeed = 5f;
        [Header("Axis")]
        [SerializeField] private Vector2 thrusterForwardAxis = Vector2.up;

        public void UpdateThrusterAnimation(Vector2 thrustVector)
        {
            var thrustDirection = thrustVector.normalized;
            var thrustPower = thrustVector.magnitude;

            for (int i = 0; i < thrusters.Count; i++)
            {
                var thruster = thrusters[i];
                Vector2 thrusterDirection = thruster.TransformDirection(thrusterForwardAxis).normalized;

                float dot = Vector2.Dot(thrusterDirection, thrustDirection);
                dot = Mathf.Clamp01(dot);

                float noise = Mathf.PerlinNoise(Time.time * thrusterNoiseSpeed, i * 56f + 12.67f);
                float noiseInfluence = noise * thrusterNoise * thrustPower;
                thruster.localScale = (noiseInfluence + dot * thrustPower) * Vector3.one;
            }
        }
    }
}
