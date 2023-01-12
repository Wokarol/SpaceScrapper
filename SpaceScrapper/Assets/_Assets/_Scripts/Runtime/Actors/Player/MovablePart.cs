using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.SpaceScrapper.Actors
{
    public class MovablePart : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body = null;
        [SerializeField] private List<Vector2> forwardAxes = new List<Vector2>() { Vector2.up };

        Vector2 forwardAxis = Vector2.zero;

        public void MoveTowards(Vector3 position, Vector2 direction)
        {
            Vector2 smoothDampVelocity = body.velocity;
            float smoothDampAngleVelocity = body.angularVelocity;

            var newAngle = Vector2.SignedAngle(forwardAxis, direction);

            body.position = Vector2.SmoothDamp(body.position, position, ref smoothDampVelocity, 0.1f);
            body.rotation = Mathf.SmoothDampAngle(body.rotation, newAngle, ref smoothDampAngleVelocity, 0.15f);
            body.velocity = smoothDampVelocity;
            body.angularVelocity = smoothDampAngleVelocity;
        }

        public void StartMove(Vector2 initialDirection)
        {
            float closestDot = -1;
            Vector2 closestDirection = Vector2.zero;
            for (int i = 0; i < forwardAxes.Count; i++)
            {
                float dot = Vector2.Dot(transform.TransformDirection(forwardAxes[i].normalized), initialDirection.normalized);

                if (dot > closestDot)
                {
                    closestDot = dot;
                    closestDirection = forwardAxes[i].normalized;
                }
            }

            if (closestDirection == Vector2.zero)
            {
                Debug.LogWarning("Could not find any direction while starting the move", this);
            }

            forwardAxis = closestDirection;
        }

        public void StopMove()
        {
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.cyan;
            for (int i = 0; i < forwardAxes.Count; i++)
            {
                Gizmos.DrawRay(Vector3.zero, forwardAxes[i]);
            }
        }
    }
}
