using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.SpaceScrapper.Actors.Components
{
    public class MovablePart : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body = null;
        [SerializeField] private List<Vector2> forwardAxes = new List<Vector2>() { Vector2.up };
        [Space]
        [SerializeField, Range(0, 1)] private float dragMultiplierForMovement = 0.2f;
        [SerializeField] private float dragMultiplierFalloffTime = 2f;

        Vector2 forwardAxis = Vector2.zero;

        float initialLinearDrag = 0;
        float initialAngularDrag = 0;

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

            body.DOKill(true);

            initialLinearDrag = body.drag;
            initialAngularDrag = body.angularDrag;

            body.drag *= dragMultiplierForMovement;
            body.angularDrag *= dragMultiplierForMovement;
        }

        public void StopMove()
        {
            DOTween.To(() => body.drag, v => body.drag = v, initialLinearDrag, dragMultiplierFalloffTime)
                .SetTarget(body)
                .SetLink(gameObject);
            DOTween.To(() => body.angularDrag, v => body.angularDrag = v, initialAngularDrag, dragMultiplierFalloffTime)
                .SetTarget(body)
                .SetLink(gameObject);
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
