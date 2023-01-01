using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;

namespace Wokarol.SpaceScrapper.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput = null;
        [SerializeField] private Rigidbody2D body = null;
        [Space]
        [SerializeField] private Transform aimPoint = null;
        [SerializeField] private float maxAimDistance = 3;
        [Space]
        [SerializeField] private ShipMovementParams movement = new();
        [Space]
        [SerializeField] private Vector2 forwardAxis = Vector2.up;

        private SceneContext sceneContext;
        private PlayerInputActions input;
        private float rotationVelocity;

        private void Start()
        {
            sceneContext = GameSystems.Get<SceneContext>();

            input = new PlayerInputActions();
            playerInput.user.AssociateActionsWithUser(input);

            input.Enable();
        }

        private void Update()
        {
            Camera mainCamera = sceneContext.MainCamera;
            var values = HandleInput(mainCamera);
            HandleMovement(values);
            PositionAimPoint(values);
        }

        private void PositionAimPoint(InputValues values)
        {
            Vector2 playerToAimVector = values.AimPointInWorldSpace - (Vector2)transform.position;
            playerToAimVector = Vector2.ClampMagnitude(playerToAimVector, maxAimDistance);
            aimPoint.position = transform.position + (Vector3)playerToAimVector;
        }

        private InputValues HandleInput(Camera camera)
        {
            if (input == null)
            {
                return InputValues.Empty;
            }

            float thrust = input.Flying.Thrust.ReadValue<float>();
            float breaking = input.Flying.Break.ReadValue<float>();
            Vector2 aimPointScreenView = input.Flying.AimPointer.ReadValue<Vector2>();

            Vector2 aimPointInWorldSpace = camera.ScreenToWorldPoint(aimPointScreenView);
            Vector2 direction = aimPointInWorldSpace - (Vector2)transform.position;

            return new InputValues(thrust, breaking, direction, aimPointInWorldSpace);
        }

        private void HandleMovement(InputValues values)
        {
            if (values.Thrust > 0)
            {
                body.AddRelativeForce(movement.ForwardThrust * values.Thrust * Vector2.up);
            }

            if (values.Break > 0)
            {
                body.AddForce(movement.BreakForce * values.Break * -body.velocity);
            }

            float newRotation = Mathf.SmoothDampAngle(body.rotation, Vector2.SignedAngle(forwardAxis, values.AimDirection), ref rotationVelocity, movement.RotationSmoothing);
            body.SetRotation(newRotation);
        }

        readonly struct InputValues
        {
            public readonly float Thrust;
            public readonly float Break;
            public readonly Vector2 AimDirection;
            public readonly Vector2 AimPointInWorldSpace;

            public InputValues(float thrust, float breaking, Vector2 aimDirection, Vector2 aimPointInWorldSpace)
            {
                Thrust = thrust;
                AimDirection = aimDirection;
                Break = breaking;
                AimPointInWorldSpace = aimPointInWorldSpace;
            }

            public static InputValues Empty => new(0, 0, Vector2.up, Vector2.up);
        }

        [System.Serializable]
        class ShipMovementParams
        {
            public float ForwardThrust = 50;
            public float BreakForce = 0.7f;
            public float RotationSmoothing = 0.2f;
        }
    }
}
