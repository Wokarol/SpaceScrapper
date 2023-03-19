using UnityEngine;
using Wokarol.Common.UI;
using Wokarol.SpaceScrapper.Actors;

namespace Wokarol.SpaceScrapper.UI.Views
{
    public class BaseCoreView : GenericBindableView<BaseCore>
    {
        [SerializeField] private UIValueBar healthBar = null;

        protected override void UpdateView()
        {
            healthBar.Value = BoundTarget.Health / (float)BoundTarget.MaxHealth;
        }

        protected override void OnBind()
        {
            healthBar.gameObject.SetActive(true);
        }

        protected override void OnUnbind(bool initialClear = false)
        {
            healthBar.gameObject.SetActive(false);
        }
    }
}
