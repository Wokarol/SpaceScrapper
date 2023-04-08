using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Global;

namespace Wokarol.SpaceScrapper.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Main Screen Buttons")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button quitGameButton;
        [Header("Disclaimer Panel")]
        [SerializeField] private PanelObjects disclaimerPanel;
        [SerializeField] private Button acceptDisclaimerPanelButton;
        [Header("New Game Panel")]
        [SerializeField] private PanelObjects newGamePanel;
        [SerializeField] private TMP_InputField gameNameInputField;
        [SerializeField] private Button newGameConfirmButton;
        [SerializeField] private Button newGameCancelButton;
        [Header("Load Game Panel")]
        [SerializeField] private PanelObjects loadGamePanel;
        [SerializeField] private Button saveButtonTemplate;
        [SerializeField] private Button loadGameCancelButton;

        private Screen screen = Screen.MainMenu;

        private void Start()
        {
            disclaimerPanel.Disable();
            newGamePanel.Disable();
            loadGamePanel.Disable();

            Time.timeScale = 1;

            newGameButton.onClick.AddListener(PressedNewGameButton);
            loadGameButton.onClick.AddListener(PressedLoadGameButton);
            quitGameButton.onClick.AddListener(PressedQuitGameButton);
            acceptDisclaimerPanelButton.onClick.AddListener(PressedAcceptDisclaimerPanelButton);
            newGameConfirmButton.onClick.AddListener(PressedNewGameConfirmButton);
            newGameCancelButton.onClick.AddListener(PressedNewGameCancelButton);
            loadGameCancelButton.onClick.AddListener(PressedLoadGameCancelButton);

            gameNameInputField.onValidateInput = GameNameInputValidate;
        }

        private char GameNameInputValidate(string text, int charIndex, char addedChar)
        {
            bool valid = char.IsLetterOrDigit(addedChar) || addedChar is ' ';
            return valid ? addedChar : '\0';
        }

        private void PressedNewGameButton()
        {
            if (screen == Screen.MainMenu)
                ChangeScreen(Screen.NewGame);
        }

        private void PressedLoadGameButton()
        {
            if (screen == Screen.MainMenu)
                ChangeScreen(Screen.LoadGame);
        }

        private void PressedQuitGameButton()
        {
            if (screen == Screen.MainMenu)
                Application.Quit();
        }

        private void PressedAcceptDisclaimerPanelButton()
        {
            if (screen == Screen.Disclaimer)
                StartGame(gameNameInputField.text);
        }

        private void PressedNewGameConfirmButton()
        {
            if (screen == Screen.NewGame)
            {
                if (string.IsNullOrWhiteSpace(gameNameInputField.text))
                {
                    gameNameInputField.transform.DOKill(true);
                    gameNameInputField.transform.DOShakePosition(1, 10);
                    return;
                }

                ChangeScreen(Screen.Disclaimer);
            }
        }

        private void PressedNewGameCancelButton()
        {
            if (screen == Screen.NewGame) 
                ChangeScreen(Screen.MainMenu);
        }

        private void PressedLoadGameCancelButton()
        {
            if (screen == Screen.LoadGame)
                ChangeScreen(Screen.MainMenu);
        }

        private void StartGame(string gameName)
        {
            Debug.Log($"Starting new game called \"{gameName}\"");
            GameSystems.Get<SceneDirector>().StartNewGame();
        }

        private void ChangeScreen(Screen screen)
        {
            var oldScreen = this.screen;
            var newScreen = screen;
            this.screen = screen;

            if (oldScreen == Screen.NewGame) newGamePanel.AnimateHide();
            if (oldScreen == Screen.LoadGame) loadGamePanel.AnimateHide();
            if (oldScreen == Screen.Disclaimer) disclaimerPanel.AnimateHide();

            if (newScreen == Screen.NewGame)
            {
                newGamePanel.AnimateShow();
                gameNameInputField.text = "";
            }

            if (newScreen == Screen.LoadGame) loadGamePanel.AnimateShow();
            if (newScreen == Screen.Disclaimer) disclaimerPanel.AnimateShow();
        }

        private enum Screen
        {
            MainMenu,
            NewGame,
            LoadGame,
            Disclaimer,
        }

        [System.Serializable]
        private struct PanelObjects
        {
            [SerializeField] private GameObject panelHolder;
            [SerializeField] private CanvasGroup fadeImage;
            [SerializeField] private RectTransform panelRect;

            internal void Disable()
            {
                panelHolder.SetActive(false);
            }

            internal void AnimateShow()
            {
                panelHolder.SetActive(true);
                fadeImage.DOFade(1, 0.2f);

                panelRect.DOKill(true);
                panelRect.DOAnchorMax(panelRect.anchorMax + Vector2.down, 0.3f).From();
                panelRect.DOAnchorMin(panelRect.anchorMin + Vector2.down, 0.3f).From();
            }

            internal void AnimateHide()
            {
                var panelHolder = this.panelHolder;
                var panelRect = this.panelRect;

                fadeImage.DOFade(0, 0.2f);

                panelRect.DOKill(true);
                var originalAnchorMax = panelRect.anchorMax;
                var originalAnchorMin = panelRect.anchorMin;
                panelRect.DOAnchorMax(panelRect.anchorMax + Vector2.down, 0.3f);
                panelRect.DOAnchorMin(panelRect.anchorMin + Vector2.down, 0.3f)
                    .OnComplete(() =>
                    {
                        panelHolder.SetActive(false);
                        panelRect.anchorMax = originalAnchorMax;
                        panelRect.anchorMin = originalAnchorMin;
                    });
            }
        }
    }
}
