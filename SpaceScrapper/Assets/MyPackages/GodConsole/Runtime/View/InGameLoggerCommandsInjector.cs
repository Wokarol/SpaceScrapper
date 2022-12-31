using UnityEngine;

namespace Wokarol.GodConsole
{
    public class InGameLoggerCommandsInjector : MonoBehaviour, IInjector
    {
        [SerializeField] private InGameConsoleLoggerView loggerView = null;

        public void Inject(GodConsole.CommandBuilder b)
        {
            b.Add("logger", (bool enable) =>
            {
                if (enable)
                {
                    loggerView.IsListening = true;
                    Debug.Log("Enabled in-game logger");
                }
                else
                {
                    Debug.Log("Disabled in-game logger");
                    loggerView.IsListening = false;
                }
            });
        }
    }
}
