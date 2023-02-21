using DG.Tweening;
using UnityEngine;
using Wokarol.SpaceScrapper.Global;

namespace Wokarol.SpaceScrapper.UI
{
    public class DirectorView : GenericBindableView<GameDirector>
    {
        [SerializeField] private CanvasGroup recallWarning;
        [SerializeField] private TMPro.TMP_Text recallTimerLabel;
        [Space]
        [SerializeField] private string pilotRecallTimerFormat = "{0:f2}";

        protected override void UpdateView()
        {
            if (BoundTarget.PlayerIsAwaitingSpawn)
            {
                if (!recallWarning.gameObject.activeSelf)
                {
                    recallWarning.DOFade(1, 0.5f);
                    recallWarning.gameObject.SetActive(true);
                }
                recallTimerLabel.text = string.Format(pilotRecallTimerFormat, BoundTarget.PlayerRespawnCountdown);
            }
            else
            {
                if (recallWarning.gameObject.activeSelf && !DOTween.IsTweening(recallWarning))
                {
                    recallWarning.DOFade(0, 0.5f)
                        .OnComplete(() => recallWarning.gameObject.SetActive(false));
                }
            }
        }

        protected override void OnUnbind(bool initialClear = false)
        {
            recallWarning.gameObject.SetActive(false);
            recallWarning.alpha = 0;
        }
    }
}
