using Wokarol.Common;
using Wokarol.Common.UI;
using Wokarol.GameSystemsLocator.Bootstrapping;
using Wokarol.GameSystemsLocator.Core;
using Wokarol.SpaceScrapper.Combat;
using Wokarol.SpaceScrapper.Global;
using Wokarol.SpaceScrapper.Saving;

namespace Wokarol.SpaceScrapper
{
    public class GameConfiguration : ISystemConfiguration
    {
        public void Configure(ServiceLocatorBuilder builder)
        {
            builder.PrefabPath = "Systems";

            builder.Add<SceneContext>(required: true);
            builder.Add<SceneLoader>(required: true);
            builder.Add<SaveSystem>(required: true);
            builder.Add<GameSettings>(required: true);
            builder.Add<GameDirector>();

            builder.Add<FancyCursor>(required: true);
            builder.Add<InputBlocker>(required: true);

            builder.Add<TargetingManager>();
        }
    }
}
