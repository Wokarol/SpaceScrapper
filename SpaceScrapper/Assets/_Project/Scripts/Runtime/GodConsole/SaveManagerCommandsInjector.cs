using UnityEngine;
using Wokarol.GodConsole;
using Wokarol.SpaceScrapper.Saving;

namespace Wokarol.SpaceScrapper.GodConsole.Injectors
{
    public class SaveManagerCommandsInjector : MonoBehaviour, IInjector
    {
        public void Inject(Wokarol.GodConsole.GodConsole.CommandBuilder b)
        {
            b.Group("game")
                .Add("save", (string name, SaveSystem saveSystem) => saveSystem.SaveGame(name))
                .Add("save", (SaveSystem saveSystem) => saveSystem.SaveGame("console"))
                .Add("load", (string name, SaveSystem saveSystem) => saveSystem.LoadGame(name))
                .Add("load", (SaveSystem saveSystem) => saveSystem.LoadGame("console"));
        }
    }
}
