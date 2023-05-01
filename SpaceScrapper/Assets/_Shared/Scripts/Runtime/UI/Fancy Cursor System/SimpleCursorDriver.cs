using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Wokarol.GameSystemsLocator;

namespace Wokarol.Common.UI
{
    public class SimpleCursorDriver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private FancyCursor.CursorType cursorType = FancyCursor.CursorType.HandPoint;
        [SerializeField] private float rotation = 0;
        [Header("Optional")]
        [SerializeField] private Selectable selectable = null;

        private bool isPointerOver;

        public void OnPointerEnter(PointerEventData eventData) => UpdatePointerOverTo(true);
        public void OnPointerExit(PointerEventData eventData) => UpdatePointerOverTo(false);
        private void OnDisable() => UpdatePointerOverTo(false);

        public void UpdatePointerOverTo(bool newIsPointerOver)
        {
            if (isPointerOver == newIsPointerOver)
                return;

            isPointerOver = newIsPointerOver;

            if (isPointerOver)
            {
                var c = GameSystems.Get<FancyCursor>();
                if (selectable == null)
                {
                    c.AddDriver(gameObject, () => new(cursorType, rotation));
                }
                else
                {
                    c.AddDriver(gameObject, () => selectable.IsInteractable()
                        ? new(cursorType, rotation)
                        : new(FancyCursor.CursorType.Default, 0));
                }
            }
            else
            {
                var c = GameSystems.Get<FancyCursor>();
                c.RemoveDriver(gameObject);
            }
        }
    }
}
