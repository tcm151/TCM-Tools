// GENERATED AUTOMATICALLY FROM 'Assets/Movement/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;


namespace TCM.Movement
{
    public class @FirstPersonControls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset {get;}

        public @FirstPersonControls()
        {
            asset = InputActionAsset.FromJson
            (
                @"{
                    ""name"": ""Controls"",
                    ""maps"": [
                        {
                            ""name"": ""FirstPerson"",
                            ""id"": ""bfa1c74a-6fc4-4c5b-a0ff-cff44d082393"",
                            ""actions"": [
                                {
                                    ""name"": ""Movement"",
                                    ""type"": ""PassThrough"",
                                    ""id"": ""40c363c7-c162-4e31-b36b-60c4102a4c2c"",
                                    ""expectedControlType"": ""Vector2"",
                                    ""processors"": """",
                                    ""interactions"": """"
                                }
                            ],
                            ""bindings"": [
                                {
                                    ""name"": ""Input"",
                                    ""id"": ""f9136014-71f0-4670-a7c9-be0a1f646181"",
                                    ""path"": ""2DVector"",
                                    ""interactions"": """",
                                    ""processors"": """",
                                    ""groups"": """",
                                    ""action"": ""Movement"",
                                    ""isComposite"": true,
                                    ""isPartOfComposite"": false
                                },
                                {
                                    ""name"": ""up"",
                                    ""id"": ""d38b19d7-994f-49c6-9a2a-0e8633278a06"",
                                    ""path"": ""<Keyboard>/w"",
                                    ""interactions"": """",
                                    ""processors"": """",
                                    ""groups"": """",
                                    ""action"": ""Movement"",
                                    ""isComposite"": false,
                                    ""isPartOfComposite"": true
                                },
                                {
                                    ""name"": ""down"",
                                    ""id"": ""c6e5f34d-921e-4213-9728-04b93af10405"",
                                    ""path"": ""<Keyboard>/s"",
                                    ""interactions"": """",
                                    ""processors"": """",
                                    ""groups"": """",
                                    ""action"": ""Movement"",
                                    ""isComposite"": false,
                                    ""isPartOfComposite"": true
                                },
                                {
                                    ""name"": ""left"",
                                    ""id"": ""85552b29-98bf-48e4-9bd7-1a86ddf8502e"",
                                    ""path"": ""<Keyboard>/a"",
                                    ""interactions"": """",
                                    ""processors"": """",
                                    ""groups"": """",
                                    ""action"": ""Movement"",
                                    ""isComposite"": false,
                                    ""isPartOfComposite"": true
                                },
                                {
                                    ""name"": ""right"",
                                    ""id"": ""cfd920e6-98a0-4867-a31a-5f4c17ee5c97"",
                                    ""path"": ""<Keyboard>/d"",
                                    ""interactions"": """",
                                    ""processors"": """",
                                    ""groups"": """",
                                    ""action"": ""Movement"",
                                    ""isComposite"": false,
                                    ""isPartOfComposite"": true
                                }
                            ]
                        }
                    ],
                    ""controlSchemes"": []
                }"
            );
            
            // FirstPerson
            m_FirstPerson = asset.FindActionMap("FirstPerson", throwIfNotFound : true);
            m_FirstPerson_Movement = m_FirstPerson.FindAction("Movement", throwIfNotFound : true);
        }

        public void Dispose() => UnityEngine.Object.Destroy(asset);

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

        public bool Contains(InputAction action) => asset.Contains(action);

        public IEnumerator<InputAction> GetEnumerator() => asset.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Enable() => asset.Enable();

        public void Disable() => asset.Disable();

        // FirstPerson
        private readonly InputActionMap m_FirstPerson;
        private IFirstPersonActions m_FirstPersonActionsCallbackInterface;
        private readonly InputAction m_FirstPerson_Movement;

        public struct FirstPersonActions
        {
            private @FirstPersonControls m_Wrapper;

            public FirstPersonActions(@FirstPersonControls wrapper)
            {
                m_Wrapper = wrapper;
            }

            public InputAction @Movement => m_Wrapper.m_FirstPerson_Movement;
            public InputActionMap Get() => m_Wrapper.m_FirstPerson;
            public void Enable() => Get().Enable();
            public void Disable() => Get().Disable();
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(FirstPersonActions set) => set.Get();

            public void SetCallbacks(IFirstPersonActions instance)
            {
                if (m_Wrapper.m_FirstPersonActionsCallbackInterface != null)
                {
                    @Movement.started -= m_Wrapper.m_FirstPersonActionsCallbackInterface.OnMovement;
                    @Movement.performed -= m_Wrapper.m_FirstPersonActionsCallbackInterface.OnMovement;
                    @Movement.canceled -= m_Wrapper.m_FirstPersonActionsCallbackInterface.OnMovement;
                }

                m_Wrapper.m_FirstPersonActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Movement.started += instance.OnMovement;
                    @Movement.performed += instance.OnMovement;
                    @Movement.canceled += instance.OnMovement;
                }
            }
        }

        public FirstPersonActions @FirstPerson => new FirstPersonActions(this);

        public interface IFirstPersonActions
        {
            void OnMovement(InputAction.CallbackContext context);
        }
    }
}