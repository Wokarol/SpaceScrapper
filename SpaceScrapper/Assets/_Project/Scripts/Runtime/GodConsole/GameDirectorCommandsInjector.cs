﻿using System.Linq;
using UnityEngine;
using Wokarol.Common;
using Wokarol.GodConsole;
using Wokarol.SpaceScrapper.Global;

namespace Wokarol.SpaceScrapper.GodConsole.Injectors
{
    public class GameDirectorCommandsInjector : MonoBehaviour, IInjector
    {
        public void Inject(Wokarol.GodConsole.GodConsole.CommandBuilder b)
        {
            b.Group("director")
                .Add("respawn", (GameDirector director) => director.ForcePlayerRespawn())
                .Add("list_spawns", (SceneContext ctx) =>
                {
                    Debug.Log(string.Join(", ", ctx.SpawnPoints.Select(p => p.name)));
                })
                .Add("skip_to_wave", (GameDirector director) => director.ForceTimerSkip());

            b.Group("player")
                .Add("respawn", (GameDirector director) => director.ForcePlayerRespawn());

            b.Group("game")
                .Add("time_scale", (float scale) => Time.timeScale = scale)
                .Add("time_scale", () => Debug.Log($"Current time scale is {Time.timeScale}"))
                .Add("name", (GameSettings set) => Debug.Log($"Current game is named: {set.GameName}"));
        }
    }
}
