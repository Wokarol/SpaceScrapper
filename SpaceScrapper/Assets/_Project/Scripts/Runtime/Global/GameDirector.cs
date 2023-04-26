using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Actors;
using Wokarol.SpaceScrapper.Actors.Components;
using Wokarol.SpaceScrapper.Saving;

namespace Wokarol.SpaceScrapper.Global
{
    public class GameDirector : MonoBehaviour
    {
        public enum GameState
        {
            Starting,
            AwaitingWave,
            SpawningWave,
            FightingWave,
            GameOver,
        }

        [Header("Player")]
        [SerializeField] private Player playerPrefab = null;
        [SerializeField] private float playerRespawnTime = 2;

        [Header("Enemy waves")]
        [SerializeField] private List<WaveDefinition> waves = new();
        [SerializeField] private int enemiesToAddEachWave = 4;

        [Header("Scene stuff to hook up")]
        [SerializeField] private Cinemachine.CinemachineVirtualCameraBase playerCamera = null;
        [SerializeField] private Cinemachine.CinemachineTargetGroup cameraAimTarget = null;


        public bool PlayerIsAwaitingSpawn { get; private set; } = false;
        public float PlayerRespawnCountdown { get; private set; } = 0;
        public float WaveCountdown { get; private set; } = 0;
        public WaveInfo CurrentWaveInformation { get; private set; } = WaveInfo.None;
        public int AliveEnemiesCount { get; private set; } = 0;
        public bool IsPaused { get; private set; } = false;

        public float TimeBetweenWaves => TimeBetweenWaves;
        public GameState CurrentGameState => gameState;
        public int WaveNumber => currentWave + 1;

        public event Action GameEnded = null;
        public event Action<bool> PauseStateChanged = null;

        private GameState gameState;
        private int currentWave = 0;

        private void Start()
        {
            StartAsync().Forget();
        }

        // The async start is moved into a separate method because Unity warnings are bad
        private async UniTaskVoid StartAsync()
        {
            ResetStateToStart();

            await GameSystems.Get<SceneDirector>().WaitUntilScenesAreReady;

            GameSystems.Get<SceneContext>().BaseCore.Destroyed += BaseCore_Destroyed;
            SpawnNewPlayerAtSuitableSpawn();

            ChangeState(GameState.AwaitingWave);
        }

        private void ResetStateToStart()
        {
            gameState = GameState.Starting;
            Time.timeScale = 1;
            AliveEnemiesCount = 0;
        }

        private void Update()
        {
            switch (gameState)
            {
                case GameState.AwaitingWave:
                    {
                        WaveCountdown -= Time.deltaTime;
                        if (WaveCountdown < 0)
                        {
                            WaveCountdown = 0;
                            GameSystems.Get<SaveSystem>().SaveGame("before-wave");
                            SpawnEnemyWave().Forget();
                        }
                    }
                    break;
                case GameState.FightingWave:
                    {
                        if (AliveEnemiesCount <= 0)
                        {
                            currentWave++;

                            ChangeState(GameState.AwaitingWave);
                        }
                    }
                    break;
            }
        }

        private async UniTask SpawnEnemyWave()
        {
            ChangeState(GameState.SpawningWave);

            int enemiesToSpawn = GetCurrentWave().enemiesToSpawn;
            CurrentWaveInformation = new WaveInfo(enemiesToSpawn);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            var spawner = GameSystems.Get<SceneContext>().WaveEnemySpawner;

            await spawner.SpawnWave(enemiesToSpawn,
                whenSpawned: p => AliveEnemiesCount++,
                whenDied: p => AliveEnemiesCount--);

            ChangeState(GameState.FightingWave);
        }

        public void ForcePlayerRespawn()
        {
            var ctx = GameSystems.Get<SceneContext>();
            ctx.Player.DestroyActor();

            SpawnPlayerAfterRecallDelay(ctx.SpawnPoints).Forget();
        }

        public void ForceTimerSkip()
        {
            WaveCountdown = 1f;
        }

        public void PauseGame()
        {
            if (CurrentGameState == GameState.GameOver)
            {
                Debug.LogError("Tried to pause the game during game over");
                return;
            }

            IsPaused = true;
            PauseStateChanged?.Invoke(IsPaused);

            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            IsPaused = false;
            PauseStateChanged?.Invoke(IsPaused);

            if (CurrentGameState == GameState.GameOver)
            {
                Debug.LogError("Tried to unpause the game during game over, game was unpasued but the timescale was unafected");
            }
            else
            {
                Time.timeScale = 1;
            }
        }

        private async UniTask SpawnPlayerAfterRecallDelay(IReadOnlyList<PlayerSpawnPosition> spawnPoints = null)
        {
            var spawnInfo = FindSuitableSpawn(spawnPoints);

            PlayerIsAwaitingSpawn = true;
            PlayerRespawnCountdown = playerRespawnTime;

            while (PlayerRespawnCountdown > 0)
            {
                PlayerRespawnCountdown -= Time.deltaTime;
                await UniTask.NextFrame(gameObject.GetCancellationTokenOnDestroy());
            }

            PlayerIsAwaitingSpawn = false;
            PlayerRespawnCountdown = -1;

            SpawnPlayerAt(spawnInfo.Position, spawnInfo.Rotation);
        }

        private void SpawnNewPlayerAtSuitableSpawn(IReadOnlyList<PlayerSpawnPosition> spawnPoints = null)
        {
            var (position, rotation) = FindSuitableSpawn(spawnPoints);
            SpawnPlayerAt(position, rotation);
        }

        private static PlayerSpawnPosition.SpawnInformation FindSuitableSpawn(IReadOnlyList<PlayerSpawnPosition> spawnPoints)
        {
            spawnPoints ??= GameSystems.Get<SceneContext>().SpawnPoints;

            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                Debug.LogWarning("Could not find a suitable spawn point, defaulting to world origin");
                return new PlayerSpawnPosition.SpawnInformation();
            }
            else
            {
                return spawnPoints[0].GetPositionAndRotation();
            }
        }

        private void SpawnPlayerAt(Vector3 pos, Quaternion rot)
        {
            var newPlayer = Instantiate(playerPrefab, pos, rot);
            SceneManager.MoveGameObjectToScene(newPlayer.gameObject, gameObject.scene);

            newPlayer.Died += Player_Died;

            AssignPlayerCamera(newPlayer);
        }

        private void Player_Died(Player obj)
        {
            SpawnPlayerAfterRecallDelay().Forget();
        }

        private void AssignPlayerCamera(Player p)
        {
            playerCamera.enabled = false;

            var playerTarget = cameraAimTarget.m_Targets[0];
            var playerAimTarget = cameraAimTarget.m_Targets[1]; // TODO: Ugly, make better

            playerTarget.target = p.transform;
            playerAimTarget.target = p.AimTarget;

            cameraAimTarget.m_Targets[0] = playerTarget;
            cameraAimTarget.m_Targets[1] = playerAimTarget;

            // The camera enabling is delayed to force it to snap to target
            // Better solution could be found later
            _ = UniTask.RunOnThreadPool(async () =>
            {
                await UniTask.NextFrame();
                playerCamera.enabled = true;
            });
        }

        private void ResetPlayerCamera()
        {
            playerCamera.enabled = false;

            // Look at: AssignPlayerCamera
            _ = UniTask.RunOnThreadPool(async () =>
            {
                await UniTask.NextFrame();
                playerCamera.enabled = true;
            });
        }

        private void BaseCore_Destroyed()
        {
            if (gameState == GameState.GameOver) return;

            Debug.Log("Exploded the reactor, game over");
            ChangeState(GameState.GameOver);
        }

        private void ChangeState(GameState newState, bool loadedState = false)
        {
            GameState oldState = gameState;
            gameState = newState;

            if (oldState == newState) return;

            // Transitions
            if (newState == GameState.GameOver)
            {
                GameEnded?.Invoke();
            }

            if (newState == GameState.AwaitingWave && !loadedState)
            {
                WaveCountdown = GetCurrentWave().timeBeforeWave;
                GameSystems.Get<SaveSystem>().SaveGame("after-wave");
            }
        }

        private WaveDefinition GetCurrentWave()
        {
            var wavesOverMax = currentWave - waves.Count + 1;
            if (wavesOverMax > 0)
            {
                var inifniteWave = waves[^1];
                inifniteWave.enemiesToSpawn += enemiesToAddEachWave * wavesOverMax;
                return inifniteWave;
            }

            return waves[currentWave];
        }

        public readonly struct WaveInfo
        {
            public readonly bool Valid;
            public readonly int SpawnedEnemyCount;

            public WaveInfo(int spawnedEnemyCount)
            {
                Valid = true;
                SpawnedEnemyCount = spawnedEnemyCount;
            }

            public static WaveInfo None => default;
        }

        [Serializable]
        public struct WaveDefinition
        {
            public int enemiesToSpawn;
            public float timeBeforeWave;
        }

        public class Memento
        {
            public float waveCoutdown;
            public GameState gameState;
            public int currentWave;

            public static Memento CreateFrom(GameDirector director)
            {
                return new Memento()
                {
                    waveCoutdown = director.WaveCountdown,
                    gameState = director.gameState,
                    currentWave = director.currentWave,
                };
            }

            public void InjectInto(GameDirector director)
            {
                director.WaveCountdown = waveCoutdown;
                director.ChangeState(gameState, true);
                director.currentWave = currentWave;
            }
        }
    }
}
