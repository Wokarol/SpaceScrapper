using Wokarol.Common;
using Wokarol.GameSystemsLocator.Bootstrapping;
using Wokarol.GameSystemsLocator.Core;
using Wokarol.SpaceScrapper.Combat;
using Wokarol.SpaceScrapper.Global;

namespace Wokarol.SpaceScrapper
{
    public class GameConfiguration : ISystemConfiguration
    {
        public void Configure(ServiceLocatorBuilder builder)
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
