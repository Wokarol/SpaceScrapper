using UnityEngine;
using Wokarol.GameSystemsLocator;
using Wokarol.GodConsole;
using Wokarol.GodConsole.View;

namespace Wokarol.Common
{
    public class ConsoleInputBlocker : MonoBehaviour, IInjector
    {
        object separateBlocker = new object();

        private void OnEnable()
        {
            if (TryGetComponent(out GodConsoleView view))
            {
                view.Showed += View_Showed;
                view.Hid += View_Hid;
            }
        }
        private void OnDisable()
        {
            if (TryGetComponent(out GodConsoleView view))
            {
                view.Showed -= View_Showed;
                view.Hid -= View_Hid;
            }
        }

        private void View_Showed()
        {
            GameSystems.Get<InputBlocker>().Block(this);
        }

        private void View_Hid()
        {
            GameSystems.Get<InputBlocker>().Unlock(this);
        }

        public void Inject(GodConsole.GodConsole.CommandBuilder b)
        {
            var blockerGroup = b.Group("input blocker");

            blockerGroup
                .Add("count", (InputBlocker blocker) => Debug.Log($"Input blocked by {blocker.Count} objects"));

            blockerGroup.Group("console")
                .Add("block", (InputBlocker blocker) =>
                {
                    blocker.Block(this);
                    Debug.Log($"Blocked input");
                })
                .Add("unblock", (InputBlocker blocker) =>
                {
                    blocker.Unlock(this);
                    Debug.Log($"Unblocked input");
                });

            blockerGroup.Group("separate")
                .Add("block", (InputBlocker blocker) =>
                {
                    blocker.Block(separateBlocker);
                    Debug.Log($"Blocked input using separate blocker");
                })
                .Add("unblock", (InputBlocker blocker) =>
                {
                    blocker.Unlock(separateBlocker);
                    Debug.Log($"Unblocked input using separate blocker");
                });
        }
    }
}
