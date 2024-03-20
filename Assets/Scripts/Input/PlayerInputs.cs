//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Scripts/Input/PlayerInputs.inputactions
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

public partial class @PlayerInputs: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputs"",
    ""maps"": [
        {
            ""name"": ""RegularControls"",
            ""id"": ""7c2a5099-c2bc-4b5b-afbf-01e1e6e67c12"",
            ""actions"": [
                {
                    ""name"": ""HorizontalMovement"",
                    ""type"": ""Value"",
                    ""id"": ""e04ce403-b029-4c1a-866b-56e60fba41cd"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""VerticalMovement"",
                    ""type"": ""Value"",
                    ""id"": ""48b9894e-8535-420c-8f8c-07ac0c2ead6c"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": ""MultiTap"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Value"",
                    ""id"": ""033cbb3d-bd37-4bea-b5f4-735adbbeb132"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Hook"",
                    ""type"": ""Button"",
                    ""id"": ""2e86db30-45a9-47e4-abbc-9580c29c7d5b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""fa30819f-e5e1-41b2-8358-5f00a27ff318"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PlungingAttack"",
                    ""type"": ""Button"",
                    ""id"": ""a2f10ddc-b371-4c6f-b3a1-367407fe4b32"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""KeyboardAxis"",
                    ""id"": ""2d32f237-008b-4425-87bf-981302cbe2ce"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""b0b236ea-1e07-4643-b133-b44692b0c78f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""edae54c6-fe7f-4645-9cf1-bb9ade908ec4"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""ControllerAxis"",
                    ""id"": ""38dec7ab-ed96-4629-bc53-4773474f2580"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""fa3502ea-c27d-4056-8b11-602602177502"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""73fc21f6-9006-42ed-9ee3-2605c82ed206"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard Axis"",
                    ""id"": ""0f42591e-f701-4826-8639-0851022cb614"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""fc434b8b-b3d3-4dd0-90c5-ffa677a2a2a5"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""709a8689-8bfa-49b3-ba0a-b48238114934"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Controller Axis"",
                    ""id"": ""ad998b1a-9e11-4762-b66c-307d9c60462c"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""02b03b87-0205-4f74-8a3f-263044569f82"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""dd812e6c-8168-4cd1-bd71-d6d8ae54e9de"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""c889d937-b7b1-42f0-970f-7ad974f0baa6"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9ccd56d7-a531-47fe-bbb7-b0f6a91099dc"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ea904493-3085-4043-b066-6035fb672d4b"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9724e1cd-d37c-460a-86b9-9d3f9bfb3708"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a7c1d438-6c36-4d63-adaf-8ce9e32b48c7"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d54dc4d2-853b-4cbb-978d-456e7981b0e3"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""328b2a8d-f70b-4c6b-bc9b-5543e14a73a0"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlungingAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""92980104-bd75-4dba-96d8-c1e1a42cdf44"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlungingAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UIControls"",
            ""id"": ""d839297e-c89a-4a4c-b776-c1e4c3353a9f"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""165b0cbe-28b4-4a68-b639-02869dc0b1dd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2a70c1ae-470f-457c-b4f5-455515eb6b17"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // RegularControls
        m_RegularControls = asset.FindActionMap("RegularControls", throwIfNotFound: true);
        m_RegularControls_HorizontalMovement = m_RegularControls.FindAction("HorizontalMovement", throwIfNotFound: true);
        m_RegularControls_VerticalMovement = m_RegularControls.FindAction("VerticalMovement", throwIfNotFound: true);
        m_RegularControls_Jump = m_RegularControls.FindAction("Jump", throwIfNotFound: true);
        m_RegularControls_Hook = m_RegularControls.FindAction("Hook", throwIfNotFound: true);
        m_RegularControls_Dash = m_RegularControls.FindAction("Dash", throwIfNotFound: true);
        m_RegularControls_PlungingAttack = m_RegularControls.FindAction("PlungingAttack", throwIfNotFound: true);
        // UIControls
        m_UIControls = asset.FindActionMap("UIControls", throwIfNotFound: true);
        m_UIControls_Newaction = m_UIControls.FindAction("New action", throwIfNotFound: true);
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

    // RegularControls
    private readonly InputActionMap m_RegularControls;
    private List<IRegularControlsActions> m_RegularControlsActionsCallbackInterfaces = new List<IRegularControlsActions>();
    private readonly InputAction m_RegularControls_HorizontalMovement;
    private readonly InputAction m_RegularControls_VerticalMovement;
    private readonly InputAction m_RegularControls_Jump;
    private readonly InputAction m_RegularControls_Hook;
    private readonly InputAction m_RegularControls_Dash;
    private readonly InputAction m_RegularControls_PlungingAttack;
    public struct RegularControlsActions
    {
        private @PlayerInputs m_Wrapper;
        public RegularControlsActions(@PlayerInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @HorizontalMovement => m_Wrapper.m_RegularControls_HorizontalMovement;
        public InputAction @VerticalMovement => m_Wrapper.m_RegularControls_VerticalMovement;
        public InputAction @Jump => m_Wrapper.m_RegularControls_Jump;
        public InputAction @Hook => m_Wrapper.m_RegularControls_Hook;
        public InputAction @Dash => m_Wrapper.m_RegularControls_Dash;
        public InputAction @PlungingAttack => m_Wrapper.m_RegularControls_PlungingAttack;
        public InputActionMap Get() { return m_Wrapper.m_RegularControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(RegularControlsActions set) { return set.Get(); }
        public void AddCallbacks(IRegularControlsActions instance)
        {
            if (instance == null || m_Wrapper.m_RegularControlsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_RegularControlsActionsCallbackInterfaces.Add(instance);
            @HorizontalMovement.started += instance.OnHorizontalMovement;
            @HorizontalMovement.performed += instance.OnHorizontalMovement;
            @HorizontalMovement.canceled += instance.OnHorizontalMovement;
            @VerticalMovement.started += instance.OnVerticalMovement;
            @VerticalMovement.performed += instance.OnVerticalMovement;
            @VerticalMovement.canceled += instance.OnVerticalMovement;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Hook.started += instance.OnHook;
            @Hook.performed += instance.OnHook;
            @Hook.canceled += instance.OnHook;
            @Dash.started += instance.OnDash;
            @Dash.performed += instance.OnDash;
            @Dash.canceled += instance.OnDash;
            @PlungingAttack.started += instance.OnPlungingAttack;
            @PlungingAttack.performed += instance.OnPlungingAttack;
            @PlungingAttack.canceled += instance.OnPlungingAttack;
        }

        private void UnregisterCallbacks(IRegularControlsActions instance)
        {
            @HorizontalMovement.started -= instance.OnHorizontalMovement;
            @HorizontalMovement.performed -= instance.OnHorizontalMovement;
            @HorizontalMovement.canceled -= instance.OnHorizontalMovement;
            @VerticalMovement.started -= instance.OnVerticalMovement;
            @VerticalMovement.performed -= instance.OnVerticalMovement;
            @VerticalMovement.canceled -= instance.OnVerticalMovement;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Hook.started -= instance.OnHook;
            @Hook.performed -= instance.OnHook;
            @Hook.canceled -= instance.OnHook;
            @Dash.started -= instance.OnDash;
            @Dash.performed -= instance.OnDash;
            @Dash.canceled -= instance.OnDash;
            @PlungingAttack.started -= instance.OnPlungingAttack;
            @PlungingAttack.performed -= instance.OnPlungingAttack;
            @PlungingAttack.canceled -= instance.OnPlungingAttack;
        }

        public void RemoveCallbacks(IRegularControlsActions instance)
        {
            if (m_Wrapper.m_RegularControlsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IRegularControlsActions instance)
        {
            foreach (var item in m_Wrapper.m_RegularControlsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_RegularControlsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public RegularControlsActions @RegularControls => new RegularControlsActions(this);

    // UIControls
    private readonly InputActionMap m_UIControls;
    private List<IUIControlsActions> m_UIControlsActionsCallbackInterfaces = new List<IUIControlsActions>();
    private readonly InputAction m_UIControls_Newaction;
    public struct UIControlsActions
    {
        private @PlayerInputs m_Wrapper;
        public UIControlsActions(@PlayerInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_UIControls_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_UIControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIControlsActions set) { return set.Get(); }
        public void AddCallbacks(IUIControlsActions instance)
        {
            if (instance == null || m_Wrapper.m_UIControlsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UIControlsActionsCallbackInterfaces.Add(instance);
            @Newaction.started += instance.OnNewaction;
            @Newaction.performed += instance.OnNewaction;
            @Newaction.canceled += instance.OnNewaction;
        }

        private void UnregisterCallbacks(IUIControlsActions instance)
        {
            @Newaction.started -= instance.OnNewaction;
            @Newaction.performed -= instance.OnNewaction;
            @Newaction.canceled -= instance.OnNewaction;
        }

        public void RemoveCallbacks(IUIControlsActions instance)
        {
            if (m_Wrapper.m_UIControlsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUIControlsActions instance)
        {
            foreach (var item in m_Wrapper.m_UIControlsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UIControlsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UIControlsActions @UIControls => new UIControlsActions(this);
    public interface IRegularControlsActions
    {
        void OnHorizontalMovement(InputAction.CallbackContext context);
        void OnVerticalMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnHook(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnPlungingAttack(InputAction.CallbackContext context);
    }
    public interface IUIControlsActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
