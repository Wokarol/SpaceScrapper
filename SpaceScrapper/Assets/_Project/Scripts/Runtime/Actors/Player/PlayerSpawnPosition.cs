using UnityEngine;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;

namespace Wokarol.SpaceScrapper.Actors.Components
{
    public class PlayerSpawnPosition : MonoBehaviour
    {
        private void Start()
        {
            GameSystems.Get<SceneContext>().AddSpawnPoint(this);
        }

        private void OnDestroy()
        {
            GameSystems.Get<SceneContext>().RemoveSpawnPoint(this);
        }

        internal SpawnInformation GetPositionAndRotation()
        {
            return new SpawnInformation()
            {
                Position = transform.position,
                Rotation = transform.rotation,
            };
        }

        public struct SpawnInformation
        {
            public Vector3 Position;
            public Quaternion Rotation;

            public void Deconstruct(out Vector3 pos, out Quaternion rot)
            {
                pos = Position;
                rot = Rotation;
            }
        }
    }
}
