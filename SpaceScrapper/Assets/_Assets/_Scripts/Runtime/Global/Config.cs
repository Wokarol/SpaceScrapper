using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;

namespace Wokarol.SpaceScrapper
{
    public class GameConfiguration : ISystemConfiguration
    {
        public void Configure(GameSystems.ConfigurationBuilder builder)
        {
            builder.PrefabPath = "Systems";

            builder.Add<SceneContext>(required: true);
            builder.Add<InputBlocker>(required: true);
            builder.Add<FancyCursor>();
        }
    }
}
