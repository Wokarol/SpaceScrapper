using UnityEngine;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Global;

namespace Wokarol.SpaceScrapper.Actors
{
    public class WarpRing : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.isTrigger == true) return;

            var player = collision.GetComponentInParent<Player>();
            if (player == null) return;

            if (Vector2.Angle(player.transform.up, transform.up) > 80f) return;

            var director = GameSystems.Get<GameDirector>();
            director.JumpToAWarpZone();
        }
    }
}
