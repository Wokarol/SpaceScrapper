using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Wokarol.Common.UI
{
    public class FancyCursor : MonoBehaviour
    {
        [SerializeField] private Image cursorImage;
        [SerializeField] private float scaleBy = 0.1f;
        [SerializeField] private float scaleDownDuration = 0.1f;
        [SerializeField] private float scaleUpDuration = 0.3f;
        [Space]
        [SerializeField] private Sprite defaultCursor;
        [SerializeField] private Sprite handPointerCursor;
        [SerializeField] private Sprite linearSlideCursor;
        [SerializeField] private Sprite rotationCursor;
        [SerializeField] private Sprite textCursor;

        private List<CursorDriver> drivers = new();
        private CursorType lastStateType;
        private Tween imageSwapTween;

        private void Start()
        {
            Cursor.visible = false;
        }

        void Update()
        {
            var screenPos = Pointer.current.position.ReadValue();
            cursorImage.transform.position = screenPos;

            if (Pointer.current.press.wasPressedThisFrame)
            {
                cursorImage.transform.DOBlendableScaleBy(Vector3.one * -scaleBy, scaleDownDuration);
            }
            if (Pointer.current.press.wasReleasedThisFrame)
            {
                cursorImage.transform.DOBlendableScaleBy(Vector3.one * scaleBy, scaleUpDuration);
            }


            CursorState state = GetCurrentState(screenPos);
            if (lastStateType != state.Type)
            {
                imageSwapTween.Kill(true);
                imageSwapTween = DOTween.Sequence()
                    .Append(cursorImage.transform.DOBlendableScaleBy(Vector3.one * 0.2f, 0.1f))
                    .AppendCallback(() =>
                    {
                        cursorImage.sprite = GetSpriteFromType(state.Type);
                    })
                    .Append(cursorImage.transform.DOBlendableScaleBy(Vector3.one * -0.2f, 0.1f))
                    .SetLink(gameObject).SetTarget(cursorImage)
                    .SetUpdate(true);
                
            }
            lastStateType = state.Type;

            //cursorImage.transform.rotation = Quaternion.Euler(0, 0, state.Rotation);

        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus) Cursor.visible = false;
        }

        public void AddDriver(GameObject caller, Func<CursorState> getter, Func<bool> isPriority = null) => AddDriver(caller, d => getter(), isPriority);
        public void AddDriver(GameObject caller, Func<CursorGetterData, CursorState> getter, Func<bool> isPriority = null)
        {
            if (drivers.Any(d => d.Owner == caller))
                return;

            drivers.Add(new()
            {
                Owner = caller,
                Getter = getter,
                IsPriorityGetter = isPriority,
            });

            if (drivers.GroupBy(c => c.Owner.transform.root).Any(g => g.Count() >= 2))
            {
                drivers.Sort((x, y) =>
                {
                    if (x.Owner.transform.IsChildOf(y.Owner.transform))
                    {
                        return -1;
                    }
                    if (y.Owner.transform.IsChildOf(x.Owner.transform))
                    {
                        return 1;
                    }
                    return 0;
                });
            }
        }

        public void RemoveDriver(GameObject caller)
        {
            int index = drivers.FindIndex(d => d.Owner == caller);

            if (index < 0)
            {
                throw new Exception($"Trying to remove a driver but none matches the caller ({caller.name})");
            }

            drivers.RemoveAt(index);
        }

        private Sprite GetSpriteFromType(CursorType type)
        {
            return type switch
            {
                CursorType.Default => defaultCursor,
                CursorType.HandPoint => handPointerCursor,
                CursorType.Linear => linearSlideCursor,
                CursorType.Circular => rotationCursor,
                CursorType.Text => textCursor,
                _ => null,
            };
        }
        private CursorState GetCurrentState(Vector2 screenPos)
        {
            if (drivers.Count == 0)
            {
                return new CursorState()
                {
                    Type = CursorType.Default,
                    Rotation = 0,
                };
            }
            else
            {
                var selectedDriver = drivers[0];

                if (drivers.Count >= 2)
                    foreach (var driver in drivers)
                    {
                        if (driver.IsPriorityGetter?.Invoke() ?? false)
                        {
                            selectedDriver = driver;
                            break;
                        }
                    }

                return selectedDriver.Getter(new()
                {
                    MousePosition = screenPos,
                });
            }
        }

        private struct CursorDriver
        {
            public GameObject Owner;
            public Func<CursorGetterData, CursorState> Getter;
            public Func<bool> IsPriorityGetter;
        }

        public struct CursorState
        {
            public CursorType Type;
            public float Rotation;

            public CursorState(CursorType type, float rotation = 0)
            {
                Type = type;
                Rotation = rotation;
            }
        }

        public struct CursorGetterData
        {
            public Vector2 MousePosition;
        }

        public enum CursorType
        {
            Default,
            HandPoint,
            Linear,
            Circular,
            Text,
        }
    }

}
