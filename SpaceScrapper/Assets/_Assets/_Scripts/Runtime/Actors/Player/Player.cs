using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Wokarol.Common;
using Wokarol.GameSystemsLocator;
using Wokarol.SpaceScrapper.Actors.Common;
using Wokarol.SpaceScrapper.Input;
using Wokarol.SpaceScrapper.Weaponry;

namespace Wokarol.SpaceScrapper.Actors
{
    public class Player : MonoBehaviour
    {
        [Header("Object References")]
        [SerializeField] private PlayerInput playerInput = null;
        [SerializeField] private SpaceshipController spaceshipController = null;
        [SerializeField] private MultiDirectionalEngineVisualController engineController = null;
        [SerializeField] private Transform aimPoint = null;
        [SerializeField] private Animator animator = null;
        [SerializeField] private Collider2D grabbingTrigger = null;
        [SerializeField] private Transform grabTarget = null;
        [SerializeField] private GunTrigger gunTrigger = null;
        [Header("Parameters")]
        [SerializeField] private float maxAimDistance = 3;
        [SerializeField] private ShipMovementParams movement = new();
        [SerializeField] private ShipMovementParams movementWhenHolding = new();
        [SerializeField] private float velocityInheritanceRatio = 0.3f;
        [Header("Animations")]
        [SerializeField] private AnimationNames animatorKeys = new();
        [Header("Grabbing")]
        [SerializeField] private LayerMask grabbableLayerMask = 0;


        private SceneContext sceneContext;
        private InputBlocker inputBlocker;
        private PlayerInputActions input;

        private InputValues lastInputValues;
        private MovementValues lastMovementValues;

        private InteractionState interactionState = InteractionState.Idle;
        private MovablePart grabbedPart = null;

        private List<Collider2D> colliderListCache = new();

        public ShipMovementParams NormalMovementParams => movement;
        public ShipMovementParams HoldingMovementParams => movementWhenHolding;

        public float VelocityInheritanceRatio { get => velocityInheritanceRatio; set => velocityInheritanceRatio = value; }

        private void Start()
        {
            sceneContext = GameSystems.Get<SceneContext>();
            inputBlocker = GameSystems.Get<InputBlocker>();

            SetupInput();
        }

        private void Update()
        {
            Camera mainCamera = sceneContext.MainCamera;
            lastInputValues = HandleInput(mainCamera, lastInputValues);
            PositionAimPoint(lastInputValues);
            engineController.UpdateThrusterAnimation(lastMovementValues.ThrustVector);

            bool wantsToShoot = interactionState != InteractionState.HoldingPart
                ? lastInputValues.WantsToShoot
                : false;

            gunTrigger.UpdateShooting(wantsToShoot, new(spaceshipController.Velocity * velocityInheritanceRatio));
        }

        private void FixedUpdate()
        {
            var shipMovementParams = interactionState == InteractionState.HoldingPart ? movementWhenHolding : movement;
            lastMovementValues = spaceshipController.HandleMovement(lastInputValues, shipMovementParams);

            if (interactionState == InteractionState.HoldingPart)
            {
                if (grabbedPart == null)
                    SwitchState(InteractionState.Idle);
                else
                    grabbedPart.MoveTowards(grabTarget.position, grabTarget.up);
            }
        }

        private void OnEnable()
        {
            input?.Enable();
        }

        private void OnDisable()
        {
            input?.Disable();
        }

        private void SetupInput()
        {
            input = new PlayerInputActions();
            playerInput.user.AssociateActionsWithUser(input);

            input.Enable();
            input.Flying.Grab.performed += Grab_performed;
        }

        private void PositionAimPoint(InputValues values)
        {
            Vector2 playerToAimVector = values.AimPointInWorldSpace - (Vector2)transform.position;
            playerToAimVector = Vector2.ClampMagnitude(playerToAimVector, maxAimDistance);
            aimPoint.position = transform.position + (Vector3)playerToAimVector;
        }

        private InputValues HandleInput(Camera camera, InputValues previousInput)
        {

            if (input == null || inputBlocker.IsBlocked)
            {
                return new InputValues(Vector2.zero, previousInput.AimDirection, false, previousInput.AimPointInWorldSpace);
            }

            Vector2 thrust = input.Flying.Move.ReadValue<Vector2>();
            thrust = Vector2.ClampMagnitude(thrust, 1);

            Vector2 aimPointScreenView = input.Flying.AimPointer.ReadValue<Vector2>();

            Vector2 aimPointInWorldSpace = camera.ScreenToWorldPoint(aimPointScreenView);
            Vector2 direction = aimPointInWorldSpace - (Vector2)transform.position;

            bool wantsToShoot = input.Flying.Shoot.IsPressed();

            return new InputValues(thrust, direction, wantsToShoot, aimPointInWorldSpace);
        }

        private void Grab_performed(InputAction.CallbackContext ctx)
        {
            if (lastInputValues.WantsToShoot) return;

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

        [System.Serializable]
        private class AnimationNames
        {
            [field: SerializeField] public string OpenWingsState { get; private set; } = "Open-Wings";
            [field: SerializeField] public string OpenWingsFailState { get; private set; } = "Open-Wings-Fail";
            [field: SerializeField] public string CloseWingsState { get; private set; } = "Close-Wings";
        }

    }
}
