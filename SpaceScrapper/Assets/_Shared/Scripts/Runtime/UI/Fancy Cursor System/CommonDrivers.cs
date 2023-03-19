using System;
using UnityEngine;
using Wokarol.GameSystemsLocator;

namespace Wokarol.Common.UI
{
    public static class LinearDriver
    {
        public static Func<FancyCursor.CursorGetterData, FancyCursor.CursorState> CreateDriverFunction(Transform transform, Vector2 localMovementAxis)
        {
            var c = GameSystems.Get<FancyCursor>();
            return d =>
            {
                var type = FancyCursor.CursorType.Linear;
                var globalMovementAxis = transform.TransformDirection(localMovementAxis);
                float angle = Vector2.SignedAngle(Vector2.up, globalMovementAxis);
                return new(type, angle);
            };
        }
    }

    public static class CircularDriver
    {
        public static Func<FancyCursor.CursorGetterData, FancyCursor.CursorState> CreateDriverFunction(Transform transform)
        {
            var ctx = GameSystems.Get<SceneContext>();
            return d =>
            {
                var type = FancyCursor.CursorType.Circular;
                Vector2 mousePos = ctx.MainCamera.ScreenToWorldPoint(d.MousePosition);
                var fromPosToMouse = mousePos - (Vector2)transform.position;
                float angle = Vector2.SignedAngle(Vector2.up, fromPosToMouse);
                return new(type, angle);
            };
        }
    }
}
