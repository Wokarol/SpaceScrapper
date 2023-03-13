using System;
using UnityEngine;
using Wokarol.Common;
using Wokarol.GodConsole;
using Wokarol.SpaceScrapper.Actors;

namespace Wokarol.SpaceScrapper.GodConsole.Injectors
{
    public class PlayerCommandsInjector : MonoBehaviour, IInjector
    {
        public void Inject(Wokarol.GodConsole.GodConsole.CommandBuilder b)
        {
            var playerGroup = b.Group("player");

            playerGroup.Group("common")
                .Add("relativity", (bool enable, SceneContext ctx) =>
                {
                    ctx.Player.NormalMovementParams.IsInputRelative = enable;
                    ctx.Player.HoldingMovementParams.IsInputRelative = enable;
                })
                .Add("relativity", (SceneContext ctx) => Debug.Log($"When normal: {ctx.Player.NormalMovementParams.IsInputRelative}, When holding: {ctx.Player.HoldingMovementParams.IsInputRelative}"));

            playerGroup.Group("physics")
                .Add("drag", (float drag, SceneContext ctx) => ctx.Player.GetComponent<Rigidbody2D>().drag = drag)
                .Add("drag", (SceneContext ctx) => Debug.Log(ctx.Player.GetComponent<Rigidbody2D>().drag))
                .Add("ang_drag", (float drag, SceneContext ctx) => ctx.Player.GetComponent<Rigidbody2D>().angularDrag = drag)
                .Add("ang_drag", (SceneContext ctx) => Debug.Log(ctx.Player.GetComponent<Rigidbody2D>().angularDrag))
                .Add("bullet_velo_inheritance", (float inheritance, SceneContext ctx) => ctx.Player.VelocityInheritanceRatio = inheritance)
                .Add("bullet_velo_inheritance", (SceneContext ctx) => Debug.Log(ctx.Player.VelocityInheritanceRatio));

            AddMovementParamsSettings(playerGroup.Group("normal"), p => p.NormalMovementParams);
            AddMovementParamsSettings(playerGroup.Group("when_holding"), p => p.HoldingMovementParams);
        }

        public void AddMovementParamsSettings(Wokarol.GodConsole.GodConsole.CommandBuilder b, Func<Player, Actors.Common.ShipMovementParams> paramsGetter)
        {
            b
                .Add("thrust", (float thrust, SceneContext ctx) => paramsGetter(ctx.Player).Thrust = thrust)
                .Add("thrust", (SceneContext ctx) => Debug.Log(paramsGetter(ctx.Player).Thrust))
                .Add("rotation_smoothing", (float smoothing, SceneContext ctx) => paramsGetter(ctx.Player).RotationSmoothing = smoothing)
                .Add("rotation_smoothing", (SceneContext ctx) => Debug.Log(paramsGetter(ctx.Player).RotationSmoothing))
                .Add("relative", (bool relative, SceneContext ctx) => paramsGetter(ctx.Player).IsInputRelative = relative)
                .Add("relative", (SceneContext ctx) => Debug.Log(paramsGetter(ctx.Player).IsInputRelative));
        }
    }
}
