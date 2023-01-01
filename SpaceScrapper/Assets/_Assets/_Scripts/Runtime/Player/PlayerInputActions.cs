//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/_Assets/Player/Player Actions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Wokarol.SpaceScrapper.Player
{
    public partial class @PlayerInputActions : IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PlayerInputActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player Actions"",
    ""maps"": [
        {
            ""name"": ""Flying"",
            ""id"": ""3451e535-c176-45ae-81ce-54bfb7ea6b49"",
            ""actions"": [
                {
                    ""name"": ""Aim (Pointer)"",
                    ""type"": ""Value"",
                    ""id"": ""8d43ed78-a7d0-46b1-b58d-29ee80dcdf0d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Thrust"",
                    ""type"": ""Value"",
                    ""id"": ""7b3d296b-7be6-4220-9f97-23275c69b4d9"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Break"",
                    ""type"": ""Value"",
                    ""id"": ""655f4d3e-a839-44ab-ba7a-7b67c0aaf160"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""412e33ca-2864-44b7-841f-0ff05a4ee94e"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim (Pointer)"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""W key"",
                    ""id"": ""4d64413a-f2d1-414f-85c2-5b4f797ef3b8"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Thrust"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""9231d71f-e60b-461a-8ab3-4949082a6d77"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Thrust"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""3e7204b6-e059-4139-b4ef-0172982370a3"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Thrust"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""S"",
                    ""id"": ""eab11838-bc34-408c-a75e-c6ffe86c7ec8"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Break"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""07825e7b-4c90-41e0-bb2d-3caefc66fba2"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Break"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""9755eef5-5143-4c74-a4c6-df268620d80c"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Break"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Flying
            m_Flying = asset.FindActionMap("Flying", throwIfNotFound: true);
            m_Flying_AimPointer = m_Flying.FindAction("Aim (Pointer)", throwIfNotFound: true);
            m_Flying_Thrust = m_Flying.FindAction("Thrust", throwIfNotFound: true);
            m_Flying_Break = m_Flying.FindAction("Break", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }
        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }
        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Flying
        private readonly InputActionMap m_Flying;
        private IFlyingActions m_FlyingActionsCallbackInterface;
        private readonly InputAction m_Flying_AimPointer;
        private readonly InputAction m_Flying_Thrust;
        private readonly InputAction m_Flying_Break;
        public struct FlyingActions
        {
            private @PlayerInputActions m_Wrapper;
            public FlyingActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @AimPointer => m_Wrapper.m_Flying_AimPointer;
            public InputAction @Thrust => m_Wrapper.m_Flying_Thrust;
            public InputAction @Break => m_Wrapper.m_Flying_Break;
            public InputActionMap Get() { return m_Wrapper.m_Flying; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(FlyingActions set) { return set.Get(); }
            public void SetCallbacks(IFlyingActions instance)
            {
                if (m_Wrapper.m_FlyingActionsCallbackInterface != null)
                {
                    @AimPointer.started -= m_Wrapper.m_FlyingActionsCallbackInterface.OnAimPointer;
                    @AimPointer.performed -= m_Wrapper.m_FlyingActionsCallbackInterface.OnAimPointer;
                    @AimPointer.canceled -= m_Wrapper.m_FlyingActionsCallbackInterface.OnAimPointer;
                    @Thrust.started -= m_Wrapper.m_FlyingActionsCallbackInterface.OnThrust;
                    @Thrust.performed -= m_Wrapper.m_FlyingActionsCallbackInterface.OnThrust;
                    @Thrust.canceled -= m_Wrapper.m_FlyingActionsCallbackInterface.OnThrust;
                    @Break.started -= m_Wrapper.m_FlyingActionsCallbackInterface.OnBreak;
                    @Break.performed -= m_Wrapper.m_FlyingActionsCallbackInterface.OnBreak;
                    @Break.canceled -= m_Wrapper.m_FlyingActionsCallbackInterface.OnBreak;
                }
                m_Wrapper.m_FlyingActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @AimPointer.started += instance.OnAimPointer;
                    @AimPointer.performed += instance.OnAimPointer;
                    @AimPointer.canceled += instance.OnAimPointer;
                    @Thrust.started += instance.OnThrust;
                    @Thrust.performed += instance.OnThrust;
                    @Thrust.canceled += instance.OnThrust;
                    @Break.started += instance.OnBreak;
                    @Break.performed += instance.OnBreak;
                    @Break.canceled += instance.OnBreak;
                }
            }
        }
        public FlyingActions @Flying => new FlyingActions(this);
        public interface IFlyingActions
        {
            void OnAimPointer(InputAction.CallbackContext context);
            void OnThrust(InputAction.CallbackContext context);
            void OnBreak(InputAction.CallbackContext context);
        }
    }
}
