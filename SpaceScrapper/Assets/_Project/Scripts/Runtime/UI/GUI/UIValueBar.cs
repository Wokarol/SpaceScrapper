using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wokarol.SpaceScrapper
{
    public class UIValueBar : MonoBehaviour
    {
        [SerializeField] private Image bar;
        [SerializeField] private Image echo;
        private float value;

        public float Value
        {
            get => value;
            set
            {
                this.value = Mathf.Clamp01(value);

                bar.fillAmount = this.value;
                echo.fillAmount = Mathf.Clamp(echo.fillAmount, bar.fillAmount, 1f);
            }
        }

        private void LateUpdate()
        {
            
        }
    }
}
