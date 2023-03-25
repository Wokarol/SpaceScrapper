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
            // REFACTOR: Consider using specialized Hide method here or something else
            OnUnbind(true);
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
                Unbind();
            }
        }

        public void BindTo(TargetT target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            BoundTarget = target;
            IsBound = true;
            OnBind();
        }

        public void Unbind()
        {
            BoundTarget = null;
            IsBound = false;
            OnUnbind();
        }

        protected virtual void UpdateView() { }
        protected virtual void OnBind() { }
        protected virtual void OnUnbind(bool initialClear = false) { }
    }
}
