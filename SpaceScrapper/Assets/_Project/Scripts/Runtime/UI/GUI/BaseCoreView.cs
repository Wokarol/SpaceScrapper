using UnityEngine;
using Wokarol.Common.UI;
using Wokarol.SpaceScrapper.Actors;

namespace Wokarol.SpaceScrapper.UI.Views
{
    public class BaseCoreView : GenericBindableView<BaseCore>
    {
        [SerializeField] private UIValueBar healthBar = null;

        override protected void UpdateView()
        {
            healthBar.Value = BoundTarget.Health / (float)BoundTarget.MaxHealth;
        }

        override protected void OnBindAndShow(bool hadTarget, bool animated)
        {
            healthBar.gameObject.SetActive(true);
        }

        override protected void OnUnbindAndHide(bool hadTarget, bool animated)
        {
            healthBar.gameObject.SetActive(false);
        }
    }
}
