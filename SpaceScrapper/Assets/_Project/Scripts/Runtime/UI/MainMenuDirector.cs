using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Global;
using Wokarol.SpaceScrapper.Saving;
using static Wokarol.SpaceScrapper.Saving.SaveSystem;

namespace Wokarol.SpaceScrapper.UI
{
    public class MainMenuDirector : MonoBehaviour
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
        [SerializeField] private SaveSlotPickerView saveSlotPickerView;
        [SerializeField] private Button loadGameCancelButton;
        [SerializeField] private Button loadGameFolderButton;

        [Space]
        [Header("Input")]
        [SerializeField] private InputAction backAction = new InputAction(name: "Back", type: InputActionType.Button);

        private Screen screen = Screen.MainMenu;
        private StartGameParams startGameParams;

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
            loadGameFolderButton.onClick.AddListener(PressedLoadGameFolderButton);

            gameNameInputField.onValidateInput = GameNameInputValidate;
            saveSlotPickerView.SelectedSavePath += SelectedLoadGameSavePath;

            backAction.Enable();
            backAction.performed += PressedBackKey;
        }

        private char GameNameInputValidate(string text, int charIndex, char addedChar)
        {
            bool valid = char.IsLetterOrDigit(addedChar) || addedChar is ' ';
            return valid ? addedChar : '\0';
        }

        private void PressedBackKey(InputAction.CallbackContext obj)
        {
            if (screen is Screen.NewGame or Screen.LoadGame)
                ChangeScreen(Screen.MainMenu);
        }

        private void PressedNewGameButton()
        {
            if (screen == Screen.MainMenu)
                ChangeScreen(Screen.NewGame);
        }

        private void PressedLoadGameButton()
        {
            if (screen == Screen.MainMenu)
            {
                ChangeScreen(Screen.LoadGame);

                var metadatas = GameSystems.Get<SaveSystem>().GetAllSaveMetdata();
                var filesGroupedBySaveName = metadatas.GroupBy(m => m.Metadata.SaveName);

                List<SaveSlotPickerView.SaveSlotOption> options = new();
                foreach (var group in filesGroupedBySaveName)
                {
                    options.Add(new(group.Key, group.OrderByDescending(nm => nm.Metadata.Date).ToList()));
                }

                saveSlotPickerView.Initialize(options);
            }
        }

        private void PressedQuitGameButton()
        {
            if (screen == Screen.MainMenu)
                Application.Quit();
        }

        private void PressedAcceptDisclaimerPanelButton()
        {
            if (screen == Screen.Disclaimer)
                StartGame();
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

                startGameParams = StartGameParams.NewGame(gameNameInputField.text);

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

        private void PressedLoadGameFolderButton()
        {
            if (screen == Screen.LoadGame)
            {
                Application.OpenURL("file:///" + SaveSystem.SaveDirectory);
            }
        }

        private void SelectedLoadGameSavePath(FileNameAndMetadata fileNameAndMetadata)
        {
            if (screen == Screen.LoadGame)
            {
                ChangeScreen(Screen.Disclaimer);
                startGameParams = StartGameParams.LoadGame(fileNameAndMetadata.Metadata.SaveName, fileNameAndMetadata.FileName);
            }

        }

        private void StartGame()
        {
            if (!startGameParams.Valid)
            {
                Debug.LogError("Tried to start the game with invalid params");
                return;
            }
            if (startGameParams.IsLoading)
            {
                Debug.Log($"Loading a game called \"{startGameParams.GameName}\" from \"{startGameParams.SavePath}\"");
                GameSystems.Get<GameSettings>().SaveFileToLoadName = startGameParams.SavePath;
                GameSystems.Get<SceneLoader>().StartGame();
            }
            else
            {
                Debug.Log($"Starting new game called \"{startGameParams.GameName}\"");
                GameSystems.Get<GameSettings>().GameName = startGameParams.GameName;
                GameSystems.Get<SceneLoader>().StartGame();
            }
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

        private readonly struct StartGameParams
        {
            public readonly bool Valid;
            public readonly string GameName;
            public readonly string SavePath;
            public readonly bool IsLoading;

            public StartGameParams(bool valid, string gameName, string savePath, bool isLoading)
            {
                Valid = valid;
                GameName = gameName;
                SavePath = savePath;
                IsLoading = isLoading;
            }

            public static StartGameParams NewGame(string name)
            {
                return new StartGameParams(true, name, "", false);
            }

            public static StartGameParams LoadGame(string name, string path)
            {
                return new StartGameParams(true, name, path, true);
            }
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
