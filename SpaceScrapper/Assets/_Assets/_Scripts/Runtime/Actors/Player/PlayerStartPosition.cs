using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;

namespace Wokarol.SpaceScrapper
{
    public class PlayerStartPosition : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return null;
            GameSystems.Get<SceneContext>().Player.TeleportTo(transform.position);
        }
    }
}
