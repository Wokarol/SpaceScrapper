using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Global;

namespace Wokarol.SpaceScrapper.UI
{
    public class DirectorView : GenericBindableView<GameDirector>
    {
        [Header("Recalling")]
        [SerializeField] private CanvasGroup recallWarning;
        [SerializeField] private TMPro.TMP_Text recallTimerLabel;
        [Space]
        [SerializeField] private string pilotRecallTimerFormat = "{0:f2}";
        [Header("Waves")]
        [SerializeField] private TMPro.TMP_Text waveLabel;
        [SerializeField] private UIValueBar waveBar;
        [Space]
        [SerializeField] private string nextWaveMessageFormat = "Next attack in {0:00.00}";
        [SerializeField] private string waveInProgressMessage = "Attack in progress!";
        [Header("Game Over screen")]
        [SerializeField] private GameObject gameOverScreen = null;
        [SerializeField] private RectTransform gameOverPanel = null;
        [Space]
        [SerializeField] private Button restartButton = null;
        [SerializeField] private Button quitButton = null;

        public bool IsGameOverShown => gameOverScreen.activeSelf;

        public event Action GameOverShown;

        protected override void Start()
        {
            base.Start();

            restartButton.onClick.AddListener(RestartGame);
            quitButton.onClick.AddListener(QuitToMenu);
        }

        private void RestartGame()
        {
            GameSystems.Get<SceneDirector>().StartNewGame();
        }

        private void QuitToMenu()
        {
            GameSystems.Get<SceneDirector>().OpenMainMenu();
        }

        protected override void UpdateView()
        {
            if (BoundTarget.PlayerIsAwaitingSpawn)
            {
                if (!recallWarning.gameObject.activeSelf)
                {
                    recallWarning.DOFade(1, 0.5f);
                    recallWarning.gameObject.SetActive(true);
                }
                recallTimerLabel.text = string.Format(pilotRecallTimerFormat, BoundTarget.PlayerRespawnCountdown);
            }
            else
            {
                if (recallWarning.gameObject.activeSelf && !DOTween.IsTweening(recallWarning))
                {
                    recallWarning.DOFade(0, 0.5f)
                        .OnComplete(() => recallWarning.gameObject.SetActive(false));
                }
            }
        }

        private void OnGameOver()
        {
            gameOverScreen.SetActive(true);

            GameOverShown?.Invoke();

            var panelSize = gameOverPanel.sizeDelta;
            panelSize.y = 0;

            DOTween.Sequence()
                .AppendCallback(() => Time.timeScale = 0f)
                .AppendInterval(0.5f)
                .Append(gameOverPanel.DOSizeDelta(panelSize, 0.75f)
                    .From()
                    .SetEase(Ease.OutBack))
                .SetUpdate(true);
        }

        protected override void OnBind()
        {
            BoundTarget.GameEnded += OnGameOver;
        }

        protected override void OnUnbind(bool initialClear = false)
        {
            if (BoundTarget != null)
                BoundTarget.GameEnded -= OnGameOver;

            recallWarning.gameObject.SetActive(false);
            recallWarning.alpha = 0;

            gameOverScreen.SetActive(false);
        }
    }
}
