using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;

namespace Wokarol.SpaceScrapper.Player
{
    public class Player : MonoBehaviour
    {
        [Header("Object References")]
        [SerializeField] private PlayerInput playerInput = null;
        [SerializeField] private Rigidbody2D body = null;
        [SerializeField] private Transform aimPoint = null;
        [SerializeField] private Animator animator = null;
        [SerializeField] private CinemachineVirtualCameraBase closeUpCamera = null;
        [Header("Parameters")]
        [SerializeField] private float maxAimDistance = 3;
        [SerializeField] private ShipMovementParams movement = new();
        [Header("Axis")]
        [SerializeField] private Vector2 forwardAxis = Vector2.up;
        [SerializeField] private Vector2 thrusterForwardAxis = Vector2.up;
        [Header("Model")]
        [SerializeField] private List<Transform> thrusters = new();
        [SerializeField] private float thrusterNoise = 0.1f;
        [SerializeField] private float thrusterNoiseSpeed = 5f;
        [Header("Animations")]
        [SerializeField] private AnimationNames animatorKeys = new();
        

        private SceneContext sceneContext;
        private PlayerInputActions input;

        private InputValues lastInputValues;
        private MovementValues lastMovementValues;

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
            lastInputValues = HandleInput(mainCamera);
            PositionAimPoint(lastInputValues);
            UpdateThrusterAnimation(lastMovementValues);

            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                animator.CrossFade(animatorKeys.OpenWingsState, 0.2f);
            }

            if (Keyboard.current.oKey.wasPressedThisFrame)
            {
                animator.CrossFade(animatorKeys.CloseWingsState, 0.2f);
            }
        }

        private void FixedUpdate()
        {
            lastMovementValues = HandleMovement(lastInputValues);
        }

        public void EnableCloseUpCamera()
        {
            if (closeUpCamera != null)
                closeUpCamera.enabled = true;
        }

        public void DisableCloseUpCamera()
        {
            if (closeUpCamera != null)
                closeUpCamera.enabled = false;
        }

        private void UpdateThrusterAnimation(MovementValues values)
        {
            var thrustDirection = values.ThrustVector.normalized;
            var thrustPower = values.ThrustVector.magnitude;

            for (int i = 0; i < thrusters.Count; i++)
            {
                var thruster = thrusters[i];
                Vector2 thrusterDirection = thruster.TransformDirection(thrusterForwardAxis).normalized;

                float dot = Vector2.Dot(thrusterDirection, thrustDirection);
                dot = Mathf.Clamp01(dot);

                float noise = Mathf.PerlinNoise(Time.time * thrusterNoiseSpeed, i * 56f + 12.67f);
                float noiseInfluence = noise * thrusterNoise * thrustPower;
                thruster.localScale = (noiseInfluence + dot * thrustPower) * Vector3.one;
            }

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

            Vector2 thrust = input.Flying.Move.ReadValue<Vector2>();
            thrust = Vector2.ClampMagnitude(thrust, 1);

            Vector2 aimPointScreenView = input.Flying.AimPointer.ReadValue<Vector2>();

            Vector2 aimPointInWorldSpace = camera.ScreenToWorldPoint(aimPointScreenView);
            Vector2 direction = aimPointInWorldSpace - (Vector2)transform.position;

            return new InputValues(thrust, direction, aimPointInWorldSpace);
        }

        private MovementValues HandleMovement(InputValues values)
        {
            var movementValues = new MovementValues();
            if (values.Thrust.magnitude > 0)
            {
                Vector2 thrustPower = values.Thrust;
                body.AddForce(movement.Thrust * thrustPower);

                movementValues.ThrustVector = thrustPower;
            }

            float targetRotation = Vector2.SignedAngle(forwardAxis, values.AimDirection);

            float currentRotation = body.rotation;
            float angVelocity = body.angularVelocity;

            float newRotation = Mathf.SmoothDampAngle(currentRotation, targetRotation, ref angVelocity, movement.RotationSmoothing);

            body.angularVelocity = angVelocity;
            body.rotation = newRotation;

            return movementValues;
        }


        readonly struct InputValues
        {
            public readonly Vector2 Thrust;
            public readonly Vector2 AimDirection;
            public readonly Vector2 AimPointInWorldSpace;

            public InputValues(Vector2 thrust, Vector2 aimDirection, Vector2 aimPointInWorldSpace)
            {
                Thrust = thrust;
                AimDirection = aimDirection;
                AimPointInWorldSpace = aimPointInWorldSpace;
            }

            public static InputValues Empty => new(Vector2.zero, Vector2.up, Vector2.up);
        }

        struct MovementValues
        {
            public Vector2 ThrustVector;
        }

        [System.Serializable]
        class AnimationNames
        {
            [field: SerializeField] public string OpenWingsState { get; private set; } = "Open-Wings";
            [field: SerializeField] public string CloseWingsState { get; private set; } = "Close-Wings";
        }

        [System.Serializable]
        class ShipMovementParams
        {
            public float Thrust = 60;
            public float RotationSmoothing = 0.2f;
        }
    }
}
