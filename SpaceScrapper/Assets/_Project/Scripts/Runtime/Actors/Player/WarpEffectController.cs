using UnityEngine;
using UnityEngine.Events;

namespace Wokarol.SpaceScrapper.Actors
{
    public interface IWarpable
    {
        void OnWarpOutApex();
        void OnWarpOutFinish();

        void OnWarpInApex();
        void OnWarpInFinish();
    }

    public class WarpEffectController : MonoBehaviour
    {
        [SerializeField] private Animator animator = null;
        [SerializeField] private string animationOutFireKey = "Warp-Out-Flash";
        [SerializeField] private string animationInFireKey = "Warp-In-Flash";
        [SerializeField] private UnityEvent onFlashInApex;
        [SerializeField] private UnityEvent onFlashOutApex;

        private IWarpable currentWarpable;

        public void PlayWarpOutAnimation(IWarpable warpable)
        {
            currentWarpable = warpable;
            animator.Play(animationOutFireKey);
        }

        public void PlayWarpInAnimation(IWarpable warpable)
        {
            currentWarpable = warpable;
            animator.Play(animationInFireKey);
        }

        public void CallFlashOutApex()
        {
            currentWarpable.OnWarpOutApex();
            onFlashOutApex.Invoke();
        }

        public void CallWarpOutFinish()
        {
            currentWarpable.OnWarpOutFinish();
        }

        public void CallFlashInApex()
        {
            currentWarpable.OnWarpInApex();
            onFlashInApex.Invoke();
        }

        public void CallWarpInFinish()
        {
            currentWarpable.OnWarpInFinish();
        }
    }
}
