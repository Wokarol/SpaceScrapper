using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Actors;
using Wokarol.SpaceScrapper.Global;

namespace Wokarol.SpaceScrapper.UI
{
    public class PlayerGUI : MonoBehaviour
    {
        [SerializeField] private UIValueBar healthBar;
        [SerializeField] private CanvasGroup outOfZoneWarning;
        [SerializeField] private CanvasGroup outOfZoneVinette;
        [SerializeField] private CanvasGroup recallWarning;
        [SerializeField] private TMPro.TMP_Text recallTimerLabel;
        [Space]
        [SerializeField] private AnimationCurve outOfZoneCountdownToVinette = AnimationCurve.Linear(0, 1, 1, 0);
        [SerializeField] private string pilotRecallTimerFormat = "{0:f2}";

        private Player boundPlayer;
        private OutOfBoundsPlayerKiller boundPlayerOutOfBoundKiller;
        private GameDirector boundDirector;

        private bool boundToPlayer;
        private bool boundToDirector;

        private void Start()
        {
            UnbindFromPlayer();
            UnbindFromDirector();
        }

        private void LateUpdate()
        {
            BindOrUnbindTheUIUsingContext();

            if (boundToPlayer)
            {
                UpdatePlayerView();
            }

            if (boundToDirector)
            {
                UpdateDirectorView();
            }
        }

        private void UpdatePlayerView()
        {
            healthBar.Value = boundPlayer.Health / (float)boundPlayer.MaxHealth;

            if (boundPlayerOutOfBoundKiller != null && boundPlayerOutOfBoundKiller.IsOutOfBounds)
            {
                if (!outOfZoneWarning.gameObject.activeSelf)
                {
                    outOfZoneWarning.DOFade(1, 0.7f);
                    outOfZoneWarning.gameObject.SetActive(true);
                }
                outOfZoneVinette.alpha = outOfZoneCountdownToVinette.Evaluate(boundPlayerOutOfBoundKiller.NormalizedKillCountdown);
            }
            else
            {
                if (outOfZoneWarning.gameObject.activeSelf && !DOTween.IsTweening(outOfZoneWarning))
                {
                    outOfZoneWarning.DOFade(0, 0.7f)
                        .OnComplete(() => outOfZoneWarning.gameObject.SetActive(false));
                }
            }
        }

        private void UpdateDirectorView()
        {
            if (boundDirector.PlayerIsAwaitingSpawn)
            {
                if (!recallWarning.gameObject.activeSelf)
                {
                    recallWarning.DOFade(1, 0.5f);
                    recallWarning.gameObject.SetActive(true);
                }
                recallTimerLabel.text = string.Format(pilotRecallTimerFormat, boundDirector.PlayerRespawnCountdown);
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

        private void BindOrUnbindTheUIUsingContext()
        {
            if (!boundToPlayer)
            {
                // Here, and event could be nice to not repeat this call too often
                var player = GameSystems.Get<SceneContext>().Player;

                if (player != null) BindToPlayer(player);
            }

            if (!boundToDirector)
            {
                var director = GameSystems.Get<GameDirector>();
                if (director != null) BindToDirector(director);
            }

            // If the bound player is destroyed, we should unbind the UI to avoid reference errors
            if (boundToPlayer && boundPlayer == null)
            {
                UnbindFromPlayer();
            }

            if (boundToDirector && boundDirector == null)
            {
                UnbindFromDirector();
            }
        }

        public void BindToPlayer(Player player)
        {
            healthBar.gameObject.SetActive(true);

            boundPlayer = player;
            boundPlayerOutOfBoundKiller = player.GetComponent<OutOfBoundsPlayerKiller>();
            boundToPlayer = true;
        }

        public void BindToDirector(GameDirector director)
        {
            boundDirector = director;
            boundToDirector = true;
        }

        public void UnbindFromPlayer()
        {
            healthBar.gameObject.SetActive(false);
            outOfZoneWarning.gameObject.SetActive(false);
            outOfZoneWarning.alpha = 0;
            outOfZoneVinette.alpha = 0;

            boundPlayer = null;
            boundToPlayer = false;
        }

        public void UnbindFromDirector()
        {
            recallWarning.gameObject.SetActive(false);
            recallWarning.alpha = 0;

            boundDirector = null;
            boundToDirector = false;
        }
    }
}
