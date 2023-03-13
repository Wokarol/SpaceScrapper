using UnityEngine;
using UnityEngine.UI;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Global;

namespace Wokarol.SpaceScrapper.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button infoButton;
        [SerializeField] private Button quitButton;
        [Space]
        [SerializeField] private GameObject infoPanel;

        private void Start()
        {
            infoPanel.SetActive(false);
            Time.timeScale = 1;

            startButton.onClick.AddListener(StartGame);
            infoButton.onClick.AddListener(OpenInfo);
            quitButton.onClick.AddListener(QuitGame);
        }

        private void StartGame()
        {
            GameSystems.Get<SceneDirector>().StartNewGame();
        }

        private void OpenInfo()
        {
            infoPanel.SetActive(true);
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}
