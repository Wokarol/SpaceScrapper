using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Actors;

namespace Wokarol.SpaceScrapper.Global
{
    public class GameDirector : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Player playerPrefab = null;
        [SerializeField] private float playerRespawnTime = 2;

        [Header("Scene stuff to hook up")]
        [SerializeField] private Cinemachine.CinemachineVirtualCameraBase playerCamera = null;
        [SerializeField] private Cinemachine.CinemachineTargetGroup cameraAimTarget = null;

        public bool PlayerIsAwaitingSpawn { get; private set; } = false;
        public float PlayerRespawnCountdown { get; private set; } = 0;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Type Safety", "UNT0006")]
        private async UniTaskVoid Start()
        {
            await UniTask.NextFrame(PlayerLoopTiming.PreUpdate);
            SpawnNewPlayerAtSuitableSpawn();
        }

        public void ForcePlayerRespawn()
        {
            var ctx = GameSystems.Get<SceneContext>();
            ctx.Player.DestroyActor();

            SpawnPlayerAfterRecallDelay(ctx.SpawnPoints).Forget();
        }

        private async UniTask SpawnPlayerAfterRecallDelay(IReadOnlyList<PlayerSpawnPosition> spawnPoints = null)
        {
            var spawnInfo = FindSuitableSpawn(spawnPoints);

            PlayerIsAwaitingSpawn = true;
            PlayerRespawnCountdown = playerRespawnTime;

            while (PlayerRespawnCountdown > 0)
            {
                PlayerRespawnCountdown -= Time.deltaTime;
                await UniTask.NextFrame();
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
            UniTask.RunOnThreadPool(async () =>
            {
                await UniTask.NextFrame();
                await UniTask.NextFrame();
                playerCamera.enabled = true;
            });
        }
    }
}
