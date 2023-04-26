using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Global;
using Wokarol.SpaceScrapper.Saving;

namespace Wokarol.SpaceScrapper.UI
{
    public class PauseMenuDirector : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button resumeButton = null;
        [SerializeField] private Button saveButton = null;
        [SerializeField] private Button quitToMenuButton = null;
        [Space]
        [SerializeField] private Button confirmationDialogYesButton = null;
        [SerializeField] private Button confirmationDialogNoButton = null;
        [SerializeField] private Button confirmationDialogCancelButton = null;
        [Header("Panels")]
        [SerializeField] private RectTransform pauseMenuPanel = null;
        [SerializeField] private RectTransform backgroundFade = null;
        [SerializeField] private RectTransform confirmationDialogPanel = null;
        [SerializeField] private TMP_Text confirmationMessageLabel = null;
        [Space]
        [TextArea, Tooltip("{elapsed} - time since the last save")]
        [SerializeField] private string confirmationMessagePattern = "{elapsed}";
        [TextArea, Tooltip("{elapsed} - time since the last save")]
        [SerializeField] private string confirmationMessageNoSavePattern = "{elapsed}";
        [Header("Input")]
        [SerializeField] private InputAction pauseAction = new InputAction("Pause Game", type: InputActionType.Button);

        private GameDirector Game => GameSystems.Get<GameDirector>();
        private bool isShowingTheConfirmationDialog = false;

        private void Start()
        {
            resumeButton.onClick.AddListener(ResumeButtonPressed);
            saveButton.onClick.AddListener(SaveButtonPressed);
            quitToMenuButton.onClick.AddListener(QuitToMenuButtonPressed);

            confirmationDialogYesButton.onClick.AddListener(ConfirmationDialogYesButtonPressed);
            confirmationDialogNoButton.onClick.AddListener(ConfirmationDialogNoButtonPressed);
            confirmationDialogCancelButton.onClick.AddListener(ConfirmationDialogCancelButtonPressed);

            pauseAction.performed += PauseAction_performed;
            pauseAction.Enable();

            UpdateVisuals();
        }

        private void OnDestroy()
        {
            pauseAction.performed -= PauseAction_performed;
            pauseAction.Disable();
        }

        private void PauseAction_performed(InputAction.CallbackContext obj)
        {
            if (GameSystems.Get<InputBlocker>().IsBlocked) return;
            if (isShowingTheConfirmationDialog) return;

            if (!Game.IsPaused)
            {
                Game.PauseGame();
            }
            else
            {
                Game.ResumeGame();
            }
            UpdateVisuals();
        }

        private void ResumeButtonPressed()
        {
            if (isShowingTheConfirmationDialog) return;

            if (Game.IsPaused)
            {
                Game.ResumeGame();
                UpdateVisuals();
            }
        }

        private void SaveButtonPressed()
        {
            if (isShowingTheConfirmationDialog) return;

            if (Game.IsPaused)
            {
                GameSystems.Get<SaveSystem>().SaveGame();
            }
        }

        private void QuitToMenuButtonPressed()
        {
            if (isShowingTheConfirmationDialog) return;

            if (Game.IsPaused)
            {
                ShowConfirmationDialog();
            }
        }

        private void ConfirmationDialogYesButtonPressed()
        {
            if (!isShowingTheConfirmationDialog) return;

            GameSystems.Get<SaveSystem>().SaveGame();
            GameSystems.Get<SceneDirector>().OpenMainMenu();
        }

        private void ConfirmationDialogNoButtonPressed()
        {
            if (!isShowingTheConfirmationDialog) return;

            GameSystems.Get<SceneDirector>().OpenMainMenu();
        }

        private void ConfirmationDialogCancelButtonPressed()
        {
            if (!isShowingTheConfirmationDialog) return;

            HideConfirmationDialog();
        }

        private void ShowConfirmationDialog()
        {
            isShowingTheConfirmationDialog = true;

            var lastSaveMetadata = GameSystems.Get<SaveSystem>().LastSaveMetadata;
            if (lastSaveMetadata.IsValid)
            {
                var lastSaveTime = lastSaveMetadata.Date;
                confirmationMessageLabel.text = confirmationMessagePattern.Replace("{elapsed}", TimeSpanToElapsed(DateTime.Now - lastSaveTime));
            }
            else
            {
                confirmationMessageLabel.text = confirmationMessageNoSavePattern.Replace("{elapsed}", TimeSpanToElapsed(Game.TimeSinceStart));
            }

            confirmationDialogPanel.gameObject.SetActive(true);
            confirmationDialogPanel.DOKill(true);
            confirmationDialogPanel.DOAnchorMax(confirmationDialogPanel.anchorMax + Vector2.down, 0.3f).From().SetUpdate(true);
            confirmationDialogPanel.DOAnchorMin(confirmationDialogPanel.anchorMin + Vector2.down, 0.3f).From().SetUpdate(true);
        }

        private string TimeSpanToElapsed(object value)
        {
            throw new NotImplementedException();
        }

        private string TimeSpanToElapsed(TimeSpan span)
        {
            List<string> elements = new();

            if (span < TimeSpan.FromSeconds(1))
                return "0s";

            if (span.Hours > 0)
                elements.Add($"{span.Hours}h");

            if (span.Minutes > 0)
                elements.Add($"{span.Minutes}m");

            if (span.Seconds > 0)
                elements.Add($"{span.Seconds}s");

            return string.Join(" ", elements);
        }

        private void HideConfirmationDialog()
        {
            isShowingTheConfirmationDialog = false;

            confirmationDialogPanel.DOKill(true);
            var originalAnchorMax = confirmationDialogPanel.anchorMax;
            var originalAnchorMin = confirmationDialogPanel.anchorMin;
            confirmationDialogPanel.DOAnchorMax(confirmationDialogPanel.anchorMax + Vector2.down, 0.3f).SetUpdate(true);
            confirmationDialogPanel.DOAnchorMin(confirmationDialogPanel.anchorMin + Vector2.down, 0.3f).SetUpdate(true)
                .OnComplete(() =>
                {
                    confirmationDialogPanel.gameObject.SetActive(false);
                    confirmationDialogPanel.anchorMax = originalAnchorMax;
                    confirmationDialogPanel.anchorMin = originalAnchorMin;
                });
        }

        private void UpdateVisuals()
        {
            bool isPaused = Game.IsPaused;
            pauseMenuPanel.gameObject.SetActive(isPaused);
            backgroundFade.gameObject.SetActive(isPaused);

            saveButton.interactable = Game.CanBeSaved;
            confirmationDialogYesButton.interactable = Game.CanBeSaved;

            if (!isPaused)
            {
                confirmationDialogPanel.gameObject.SetActive(false);
            }
        }
    }
}
