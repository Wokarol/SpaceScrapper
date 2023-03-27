using System;
using UnityEngine;

namespace Wokarol.Common.UI
{
    public abstract class GenericBindableView<TargetT> : MonoBehaviour where TargetT : Component
    {
        public TargetT BoundTarget { get; private set; }
        public bool IsBound { get; private set; }

        protected virtual void Start()
        {
            UnbindAndHide(false);
        }

        private void LateUpdate()
        {
            UnbindIfTargetIsDestroyed();

            if (IsBound)
            {
                UpdateView();
            }
        }

        private void UnbindIfTargetIsDestroyed()
        {
            if (IsBound && BoundTarget == null)
            {
                UnbindAndHide();
            }
        }

        public void BindAndShow(TargetT target, bool animated = true)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            bool hadTarget = BoundTarget != null;

            BoundTarget = target;
            IsBound = true;

            OnBindAndShow(hadTarget, animated);
        }

        public void UnbindAndHide(bool animated = true)
        {
            bool hadTarget = BoundTarget != null;

            BoundTarget = null;
            IsBound = false;

            OnUnbindAndHide(hadTarget, animated);
        }

        protected virtual void UpdateView() { }
        protected virtual void OnBindAndShow(bool hadTarget, bool animated) { }
        protected virtual void OnUnbindAndHide(bool hadTarget, bool animated) { }
    }
}
