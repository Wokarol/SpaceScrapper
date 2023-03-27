using DG.Tweening;
using UnityEngine;
using Wokarol.Common.UI;
using Wokarol.SpaceScrapper.Actors;
using Wokarol.SpaceScrapper.Actors.PlayBounds;

namespace Wokarol.SpaceScrapper.UI.Views
{
    public class PlayerView : GenericBindableView<Player>
    {
        [SerializeField] private UIValueBar healthBar;
        [SerializeField] private CanvasGroup outOfZoneWarning;
        [SerializeField] private CanvasGroup outOfZoneVinette;
        [Space]
        [SerializeField] private AnimationCurve outOfZoneCountdownToVinette = AnimationCurve.Linear(0, 1, 1, 0);

        private OutOfBoundsPlayerKiller boundPlayerOutOfBoundKiller;

        protected override void UpdateView()
        {
            healthBar.Value = BoundTarget.Health / (float)BoundTarget.MaxHealth;

            if (boundPlayerOutOfBoundKiller != null && boundPlayerOutOfBoundKiller.IsOutOfBounds)
            {
                if (!outOfZoneWarning.gameObject.activeSelf)
                {
                    outOfZoneWarning.DOFade(1, 0.7f);
                    outOfZoneWarning.gameObject.SetActive(true);
                }
                outOfZoneVinette.alpha = outOfZoneCountdownToVinette.Evaluate(boundPlayerOutOfBoundKiller.NormalizedKillCountdown);
            }
            else
            {
                if (outOfZoneWarning.gameObject.activeSelf && !DOTween.IsTweening(outOfZoneWarning))
                {
                    outOfZoneWarning.DOFade(0, 0.7f)
                        .OnComplete(() => outOfZoneWarning.gameObject.SetActive(false));
                }
            }
        }


        protected override void OnBindAndShow(bool hadTarget, bool animated)
        {
            healthBar.gameObject.SetActive(true);

            boundPlayerOutOfBoundKiller = BoundTarget.GetComponent<OutOfBoundsPlayerKiller>();
        }

        protected override void OnUnbindAndHide(bool hadTarget, bool animated)
        {
            healthBar.gameObject.SetActive(false);
            outOfZoneWarning.gameObject.SetActive(false);
            outOfZoneWarning.alpha = 0;
            outOfZoneVinette.alpha = 0;
        }
    }
}
