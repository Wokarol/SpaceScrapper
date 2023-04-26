using UnityEngine;

namespace Wokarol.SpaceScrapper.Global
{
    public class GameSettings : MonoBehaviour
    {
        public string GameName { get; set; } = "Test";
        public string LoadedSaveFileName { get; set; } = null;

        public bool ShouldStartFromAFile => LoadedSaveFileName != null;
    }
}
