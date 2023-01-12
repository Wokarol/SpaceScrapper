using UnityEngine;
using UnityEngine.InputSystem;

namespace Wokarol.SpaceScrapper.Actors.Common
{
    public class SpaceshipController : MonoBehaviour
    {
        [Header("Object References")]
        [SerializeField] private Rigidbody2D body = null;

        [Header("Axis")]
        [SerializeField] private Vector2 forwardAxis = Vector2.up;

        public Vector2 Velocity => body.velocity;
        public Vector2 Forward => transform.TransformDirection(forwardAxis);

        public MovementValues HandleMovement(InputValues values, ShipMovementParams movementParams)
        {
            var movementValues = new MovementValues();

            if (values.Thrust.magnitude > 0)
            {
                Vector2 thrustPower = values.Thrust;
                if (movementParams.IsInputRelative)
                {
                    body.AddRelativeForce(movementParams.Thrust * thrustPower);
                    movementValues.ThrustVector = transform.rotation * thrustPower;
                }
                else
                {
                    body.AddForce(movementParams.Thrust * thrustPower);
                    movementValues.ThrustVector = thrustPower;
                }
            }

            float targetRotation = Vector2.SignedAngle(forwardAxis, values.AimDirection);

            float currentRotation = body.rotation;
            float angVelocity = body.angularVelocity;

            float newRotation = Mathf.SmoothDampAngle(currentRotation, targetRotation, ref angVelocity, movementParams.RotationSmoothing);

            body.angularVelocity = angVelocity;
            body.rotation = newRotation;

            return movementValues;
        }
    }

    public readonly struct InputValues
    {
        public readonly Vector2 Thrust;
        public readonly Vector2 AimDirection;
        public readonly Vector2 AimPointInWorldSpace;
        public readonly bool WantsToShoot;

        public InputValues(Vector2 thrust, Vector2 aimDirection, bool wantsToShoot = false, Vector2 aimPointInWorldSpace = new())
        {
            Thrust = thrust;
            AimDirection = aimDirection;
            AimPointInWorldSpace = aimPointInWorldSpace;
            WantsToShoot = wantsToShoot;
        }

        public static InputValues Empty => new(Vector2.zero, Vector2.up);
    }

    public struct MovementValues
    {
        public Vector2 ThrustVector;
    }

    [System.Serializable]
    public class ShipMovementParams
    {
        public float Thrust = 60;
        public float RotationSmoothing = 0.2f;
        public bool IsInputRelative = false;
    }
}
