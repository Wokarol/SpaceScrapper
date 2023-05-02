using UnityEngine;
using UnityEngine.Events;

namespace Wokarol.SpaceScrapper.Actors
{
    public interface IWarpable
    {
        void OnWarpApex();
        void OnWarpFinish();
    }

    public class WarpEffectController : MonoBehaviour
    {
        [SerializeField] private Animator animator = null;
        [SerializeField] private string animationFireKey = "Warp-Flash";
        [SerializeField] private UnityEvent onFlashApex;

        private IWarpable currentWarpable;

        public void PlayWarpAnimation(IWarpable warpable)
        {
            currentWarpable = warpable;
            animator.Play(animationFireKey);
        }

        public void CallFlashApex()
        {
            currentWarpable.OnWarpApex();
            onFlashApex.Invoke();
        }

        public void CallWarpFinish()
        {
            currentWarpable.OnWarpFinish();
        }
    }
}
