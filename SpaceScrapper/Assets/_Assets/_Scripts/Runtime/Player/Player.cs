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
        [SerializeField] private Collider2D grabbingTrigger = null;
        [SerializeField] private Transform grabTarget = null;
        [Header("Parameters")]
        [SerializeField] private float maxAimDistance = 3;
        [SerializeField] private ShipMovementParams movement = new();
        [SerializeField] private ShipMovementParams movementWhenHolding = new();
        [Header("Axis")]
        [SerializeField] private Vector2 forwardAxis = Vector2.up;
        [SerializeField] private Vector2 thrusterForwardAxis = Vector2.up;
        [Header("Model")]
        [SerializeField] private List<Transform> thrusters = new();
        [SerializeField] private float thrusterNoise = 0.1f;
        [SerializeField] private float thrusterNoiseSpeed = 5f;
        [Header("Animations")]
        [SerializeField] private AnimationNames animatorKeys = new();
        [Header("Grabbing")]
        [SerializeField] private LayerMask grabbableLayerMask = 0;


        private SceneContext sceneContext;
        private PlayerInputActions input;

        private InputValues lastInputValues;
        private MovementValues lastMovementValues;

        private InteractionState interactionState = InteractionState.Idle;
        private MovablePart grabbedPart = null;

        private List<Collider2D> colliderListCache = new();


        private void Start()
        {
            sceneContext = GameSystems.Get<SceneContext>();

            SetupInput();
        }

        private void Update()
        {
            Camera mainCamera = sceneContext.MainCamera;
            lastInputValues = HandleInput(mainCamera);
            PositionAimPoint(lastInputValues);
            UpdateThrusterAnimation(lastMovementValues);
        }

        private void FixedUpdate()
        {
            lastMovementValues = HandleMovement(lastInputValues);

            if (interactionState == InteractionState.HoldingPart)
            {
                grabbedPart.MoveTowards(grabTarget.position, grabTarget.up);
            }
        }

        private void OnEnable()
        {
            if (input != null)
                input.Enable();
        }

        private void OnDisable()
        {
            if (input != null)
                input.Disable();
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

        private void SetupInput()
        {
            input = new PlayerInputActions();
            playerInput.user.AssociateActionsWithUser(input);

            input.Enable();
            input.Flying.Grab.performed += Grab_performed;
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

        private void Grab_performed(InputAction.CallbackContext ctx)
        {
            bool isPressed = ctx.ReadValueAsButton();

            switch (interactionState)
            {
                case InteractionState.Idle when isPressed:
                    TryToGrabPart();
                    break;

                case InteractionState.HoldingPart when !isPressed:
                    SwitchState(InteractionState.Idle);
                    break;
            }
        }

        private void TryToGrabPart()
        {
            grabbingTrigger.OverlapCollider(new ContactFilter2D()
            {
                layerMask = grabbableLayerMask,
                useLayerMask = true,
            }, colliderListCache);

            float closestDistance = float.PositiveInfinity;
            MovablePart closestPart = null;

            foreach (var collider in colliderListCache)
            {
                if (collider.TryGetComponent(out MovablePart part))
                {
                    float distance = Vector2.Distance(grabTarget.position, part.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPart = part;
                    }
                }
            }

            if (closestPart != null)
            {
                grabbedPart = closestPart;
                SwitchState(InteractionState.HoldingPart);
            }
            else
            {
                animator.CrossFade(animatorKeys.OpenWingsFailState, 0.1f);
            }
        }

        private MovementValues HandleMovement(InputValues values)
        {
            var movementValues = new MovementValues();
            var movementParams = interactionState == InteractionState.HoldingPart ? movementWhenHolding : movement;

            if (values.Thrust.magnitude > 0)
            {
                Vector2 thrustPower = values.Thrust;
                body.AddForce(movementParams.Thrust * thrustPower);

                movementValues.ThrustVector = thrustPower;
            }

            float targetRotation = Vector2.SignedAngle(forwardAxis, values.AimDirection);

            float currentRotation = body.rotation;
            float angVelocity = body.angularVelocity;

            float newRotation = Mathf.SmoothDampAngle(currentRotation, targetRotation, ref angVelocity, movementParams.RotationSmoothing);

            body.angularVelocity = angVelocity;
            body.rotation = newRotation;

            return movementValues;
        }

        private void SwitchState(InteractionState newState)
        {
            var lastState = interactionState;
            interactionState = newState;

            if (lastState == interactionState)
                return;

            if (lastState == InteractionState.HoldingPart)
            {
                if (grabbedPart != null)
                {
                    grabbedPart.StopMove();
                    grabbedPart = null;
                }

                animator.CrossFade(animatorKeys.CloseWingsState, 0.2f);
            }

            if (newState == InteractionState.HoldingPart)
            {
                animator.CrossFade(animatorKeys.OpenWingsState, 0.2f);

                if (grabbedPart == null)
                    Debug.LogWarning($"You should not enter the {nameof(InteractionState.HoldingPart)} state without setting {grabbedPart}", this);
                else
                    grabbedPart.StartMove(grabTarget.up);
            }
        }

        private enum InteractionState
        {
            Idle,
            HoldingPart,
        }

        private readonly struct InputValues
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

        private struct MovementValues
        {
            public Vector2 ThrustVector;
        }

        [System.Serializable]
        private class AnimationNames
        {
            [field: SerializeField] public string OpenWingsState { get; private set; } = "Open-Wings";
            [field: SerializeField] public string OpenWingsFailState { get; private set; } = "Open-Wings-Fail";
            [field: SerializeField] public string CloseWingsState { get; private set; } = "Close-Wings";
        }

        [System.Serializable]
        private class ShipMovementParams
        {
            public float Thrust = 60;
            public float RotationSmoothing = 0.2f;
        }
    }
}
