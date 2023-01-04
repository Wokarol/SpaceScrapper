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
        [Header("Parameters")]
        [SerializeField] private float maxAimDistance = 3;
        [SerializeField] private ShipMovementParams movement = new();
        [Header("Axis")]
        [SerializeField] private Vector2 forwardAxis = Vector2.up;
        [SerializeField] private Vector2 breakThrusterForwardAxis = Vector2.up;
        [Header("Model")]
        [SerializeField] private List<Transform> mainThrusters = new();
        [SerializeField] private List<Transform> breakThrusters = new();
        [SerializeField] private float thrusterNoise = 0.1f;
        [SerializeField] private float thrusterNoiseSpeed = 5f;
        [SerializeField] private float breakingForceMaxVisualMagnitude = 1f;

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

            //if (Keyboard.current.pKey.wasPressedThisFrame)
            //    rotationVelocity = 0f;

            if (Keyboard.current.leftBracketKey.wasPressedThisFrame)
                body.angularVelocity = 0f;

            if (Keyboard.current.oKey.wasPressedThisFrame)
            {
                body.rotation += 90f;
            }

            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                body.rotation -= 90f;
            }
        }

        private void FixedUpdate()
        {
            lastMovementValues = HandleMovement(lastInputValues);
        }

        private void UpdateThrusterAnimation(MovementValues values)
        {
            for (int i = 0; i < mainThrusters.Count; i++)
            {
                float noise = Mathf.PerlinNoise(Time.time * thrusterNoiseSpeed, i * 56f + 12.67f);
                float noiseInfluence = noise * thrusterNoise * values.MainThrusterPower;
                mainThrusters[i].localScale = (noiseInfluence + values.MainThrusterPower) * Vector3.one;
            }

            Vector2 breakingDirection = -values.BreakMgnitudeInWorldSpace.normalized;
            float breakPower = values.BreakPower * Mathf.Clamp01(Mathf.InverseLerp(0, breakingForceMaxVisualMagnitude, values.BreakMgnitudeInWorldSpace.magnitude));

            for (int i = 0; i < breakThrusters.Count; i++)
            {
                var thruster = breakThrusters[i];
                Vector2 thrusterDirection = thruster.TransformDirection(breakThrusterForwardAxis).normalized;

                float dot = Vector2.Dot(thrusterDirection, breakingDirection);
                dot = Mathf.Clamp01(dot);
                dot *= dot;

                float noise = Mathf.PerlinNoise(Time.time * thrusterNoiseSpeed, i * 56f + 12.67f);
                float noiseInfluence = noise * thrusterNoise * breakPower;

                thruster.localScale = (noiseInfluence + dot * breakPower) * Vector3.one;
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

            float thrust = input.Flying.Thrust.ReadValue<float>();
            float breaking = input.Flying.Break.ReadValue<float>();
            Vector2 aimPointScreenView = input.Flying.AimPointer.ReadValue<Vector2>();

            Vector2 aimPointInWorldSpace = camera.ScreenToWorldPoint(aimPointScreenView);
            Vector2 direction = aimPointInWorldSpace - (Vector2)transform.position;

            return new InputValues(thrust, breaking, direction, aimPointInWorldSpace);
        }

        private MovementValues HandleMovement(InputValues values)
        {
            var movementValues = new MovementValues();
            if (values.Thrust > 0)
            {
                float thrustPower = values.Thrust;
                body.AddRelativeForce(movement.ForwardThrust * thrustPower * Vector2.up);

                movementValues.MainThrusterPower = thrustPower;
            }

            if (values.Break > 0)
            {
                float breakPower = values.Break;
                var breakDirection = -body.velocity;
                body.AddForce(movement.BreakForce * breakPower * breakDirection);

                movementValues.BreakPower = breakPower;
                movementValues.BreakMgnitudeInWorldSpace = breakDirection;
            }

            float targetRotation = Vector2.SignedAngle(forwardAxis, values.AimDirection);

            float currentRotation = body.rotation;
            float angVelocity = body.angularVelocity;

            float newRotation = Mathf.SmoothDampAngle(currentRotation, targetRotation, ref angVelocity, movement.RotationSmoothing);

            body.angularVelocity = angVelocity;
            //float newRotation = targetRotation;

            Debug.DrawRay(transform.position, Quaternion.AngleAxis(targetRotation, Vector3.forward) * Vector3.up * 5f, Color.green);
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(currentRotation, Vector3.forward) * Vector3.up * 5f, Color.yellow);

            body.rotation = newRotation;

            DEBUG_targetRotation = targetRotation;
            DEBUG_currentRotation = currentRotation;
            DEBUG_newRotation = newRotation;

            return movementValues;
        }

        private float DEBUG_targetRotation;
        private float DEBUG_currentRotation;
        private float DEBUG_newRotation;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(30, 30, 600, 300));

            GUILayout.Label($"Current: \t{DEBUG_currentRotation:f3}");
            GUILayout.Label($"Target: \t{DEBUG_targetRotation:f3}");
            GUILayout.Label($"New: \t{DEBUG_newRotation:f3}");
            GUILayout.Space(10);
            GUILayout.Label($"Velocity: \t{body.angularVelocity:f3}");

            GUILayout.EndArea();
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

        struct MovementValues
        {
            public float MainThrusterPower;
            public float BreakPower;
            public Vector2 BreakMgnitudeInWorldSpace;
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
