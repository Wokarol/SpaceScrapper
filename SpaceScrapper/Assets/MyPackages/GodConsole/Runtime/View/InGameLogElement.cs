using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Wokarol.GodConsole
{
    public class InGameLogElement : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Image indicator;
        [Space]
        [SerializeField] private Color infoColor;
        [SerializeField] private Color warnColor;
        [SerializeField] private Color errorColor;

        public void Show(string message, LogType type)
        {
            gameObject.SetActive(true);

            label.text = message;

            indicator.color = type switch
            {
                LogType.Log => infoColor,
                LogType.Warning => warnColor,
                LogType.Assert => errorColor,
                LogType.Exception => errorColor,
                LogType.Error => errorColor,
                _ => infoColor
            };

            group.alpha = 0.0f;
            group.DOFade(1f, 0.4f).SetLink(gameObject);
        }
    }
}
