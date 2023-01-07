using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.Common;
using Wokarol.GodConsole;

namespace Wokarol.SpaceScrapper.GodConsole.Injectors
{
    public class PlayerCommandsInjector : MonoBehaviour, IInjector
    {
        public void Inject(Wokarol.GodConsole.GodConsole.CommandBuilder b)
        {
            var playerGroup = b.Group("player");

            playerGroup.Group("input")
                .Add("relativity", (bool enable, SceneContext ctx) => ctx.Player.IsInputRelative = enable)
                .Add("relativity", (SceneContext ctx) => Debug.Log(ctx.Player.IsInputRelative));
        }
    }
}
