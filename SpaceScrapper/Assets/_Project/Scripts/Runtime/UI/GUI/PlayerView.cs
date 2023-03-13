using DG.Tweening;
using UnityEngine;
using Wokarol.SpaceScrapper.Actors;

namespace Wokarol.SpaceScrapper.UI
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


        override protected void OnBind()
        {
            healthBar.gameObject.SetActive(true);

            boundPlayerOutOfBoundKiller = BoundTarget.GetComponent<OutOfBoundsPlayerKiller>();
        }

        override protected void OnUnbind(bool initialClear = false)
        {
            healthBar.gameObject.SetActive(false);
            outOfZoneWarning.gameObject.SetActive(false);
            outOfZoneWarning.alpha = 0;
            outOfZoneVinette.alpha = 0;
        }
    }
}
