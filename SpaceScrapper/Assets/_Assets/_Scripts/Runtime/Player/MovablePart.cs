using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.SpaceScrapper.Player
{
    public class MovablePart : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body = null;
        [SerializeField] private List<Vector2> forwardAxes = new List<Vector2>() { Vector2.up };
        private Vector2 smoothDampVelocity;
        private float smoothDampAngleVelocity;

        Vector2 forwardAxis = Vector2.zero;

        public void MoveTowards(Vector3 position, Vector2 direction)
        {
            body.position = Vector2.SmoothDamp(body.position, position, ref smoothDampVelocity, 0.05f);

            Debug.DrawRay(transform.position, forwardAxis * 2f);
            Debug.DrawRay(transform.position, direction * 2f);

            var newAngle = Vector2.SignedAngle(forwardAxis, direction);

            body.rotation = Mathf.SmoothDampAngle(body.rotation, newAngle, ref smoothDampAngleVelocity, 0.05f);
        }

        public void StartMove(Vector2 initialDirection)
        {
            body.isKinematic = true;
            body.velocity = Vector3.zero;
            body.angularVelocity = 0;

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
            body.isKinematic = false;
            body.velocity = smoothDampVelocity;
            body.angularVelocity = smoothDampAngleVelocity;
        }
    }
}
