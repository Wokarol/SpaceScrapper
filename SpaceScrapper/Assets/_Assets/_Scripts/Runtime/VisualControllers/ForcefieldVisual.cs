using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wokarol.SpaceScrapper
{
    public class ForcefieldVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer forcefieldWave = null;
        [SerializeField] private float cycleLength = 0.5f;
        [SerializeField] private float scaleIncrease = 0.3f;
        [SerializeField] private int waveCount = 3;
        [Space]
        [SerializeField] private AnimationCurve alphaOverTime = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve scaleMultiplyOverTime = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private bool invertScaleAnimation = false;

        float t = 0;
        private List<SpriteRenderer> forcefieldWaves = new();

        private void Start()
        {
            forcefieldWaves.Clear();

            forcefieldWaves.Add(forcefieldWave);
            for (int i = 0; i < waveCount - 1; i++)
            {
                var ob = Instantiate(forcefieldWave, forcefieldWave.transform.parent);
                forcefieldWaves.Add(ob);
            }
        }

        private void OnEnable()
        {
            t = 0;
        }

        private void Update()
        {
            t += Time.deltaTime;
            float waveInterval = 1f / forcefieldWaves.Count;

            for (int i = 0; i < forcefieldWaves.Count; i++)
            {
                float normalizedTime = t / cycleLength;
                normalizedTime -= waveInterval * i;

                if (normalizedTime < 0)
                {
                    forcefieldWaves[i].transform.localScale = Vector3.zero;
                    continue;
                }

                float posInCycle = Mathf.Repeat(normalizedTime, 1f);

                float rawScaleMultiplier = posInCycle;
                if (invertScaleAnimation)
                    rawScaleMultiplier = 1f - rawScaleMultiplier;

                float scaleMultiplier = scaleMultiplyOverTime.Evaluate(rawScaleMultiplier);
                forcefieldWaves[i].transform.localScale = (1f + scaleIncrease * scaleMultiplier) * Vector3.one;

                Color c = forcefieldWaves[i].color;
                c.a = alphaOverTime.Evaluate(posInCycle);
                forcefieldWaves[i].color = c;
            }
        }
    }
}
