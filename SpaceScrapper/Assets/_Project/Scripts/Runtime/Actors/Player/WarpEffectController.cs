using UnityEngine;
using UnityEngine.Events;

namespace Wokarol.SpaceScrapper.Actors
{
    public class WarpEffectController : MonoBehaviour
    {
        [SerializeField] private Animator animator = null;
        [SerializeField] private string animationFireKey = "Warp-Flash";
        [SerializeField] private UnityEvent onFlashApex;

        public void PlayWarpAnimation()
        {
            animator.Play(animationFireKey);
        }

        public void CallFlashApex()
        {
            onFlashApex.Invoke();
        }
    }
}
