using Wokarol.Common;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Combat;
using Wokarol.SpaceScrapper.Global;

namespace Wokarol.SpaceScrapper
{

    public class GameConfiguration : ISystemConfiguration
    {
        public void Configure(GameSystems.ConfigurationBuilder builder)
        {
            builder.PrefabPath = "Systems";

            builder.Add<SceneContext>(required: true);
            builder.Add<InputBlocker>(required: true);
            builder.Add<SceneDirector>(required: true);
            builder.Add<FancyCursor>(required: true);
            builder.Add<GameDirector>();
            builder.Add<TargetingManager>();
        }
    }
}
