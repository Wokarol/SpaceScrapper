using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Wokarol.Common.Other
{
    public class SetVolumeWeigthToAlpha : MonoBehaviour
    {

        private CanvasGroup group;
        private Volume volume;

        private void Start()
        {
            if (!TryGetComponent(out group)) Debug.LogWarning($"There is no {nameof(CanvasGroup)} component!", this);
            if (!TryGetComponent(out volume)) Debug.LogWarning($"There is no {nameof(Volume)} component!", this);
        }

        private void Update()
        {
            volume.weight = group.alpha;
        }
    }
}
