using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.SpaceScrapper.EyeCandy
{
    public class PulseCanvasGroupAlpha : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group = null;
        [SerializeField] private float interval = 4f;
        [SerializeField] private float fadeInDuration = 0.05f;
        [SerializeField] private float fadeOutDuration = 0.2f;

        float counter = 0;

        private void Start()
        {
            group.alpha = 0;
        }

        private void Update()
        {
            counter += Time.deltaTime;
            if (counter > interval)
            {
                counter -= interval;

                Pulse();
            }
        }

        private void Pulse()
        {
            DOTween.Sequence()
                .Append(group.DOFade(1, fadeInDuration))
                .Append(group.DOFade(0, fadeOutDuration))
                .SetLink(gameObject).SetId("Canvas Group Pulsing");
        }
    }
}
