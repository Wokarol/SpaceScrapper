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
        [SerializeField] private Player playerPrefab = null;

        [Header("Scene stuff to hook up")]
        [SerializeField] private Cinemachine.CinemachineVirtualCameraBase playerCamera = null;
        [SerializeField] private Cinemachine.CinemachineTargetGroup cameraAimTarget = null;


        private void Start()
        {
            SpawnNewPlayerAtSuitableSpawn();
        }

        public void RespawnPlayer()
        {
            var ctx = GameSystems.Get<SceneContext>();
            ctx.Player.DestroyActor();
            SpawnNewPlayerAtSuitableSpawn(ctx.SpawnPoints);
        }

        private void SpawnNewPlayerAtSuitableSpawn(IReadOnlyList<PlayerSpawnPosition> spawnPoints = null)
        {
            if (spawnPoints == null) 
                spawnPoints = GameSystems.Get<SceneContext>().SpawnPoints;


            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;

            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                Debug.LogWarning("Could not find a suitable spawn point, defaulting to world origin");
            }
            else
            {
                (position, rotation) = spawnPoints[0].GetPositionAndRotation();
            }


            SpawnPlayerAt(position, rotation);
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
            SpawnNewPlayerAtSuitableSpawn();
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
