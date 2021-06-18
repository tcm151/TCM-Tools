using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace TCM.Movement
{
    public class FirstPersonView : MonoBehaviour
    {
        //+ COMPONENTS
        new public Camera camera;
        public Transform player, view;
        
        //+ CONTROL DEVICES
        private Mouse mouse;
        private Keyboard keyboard;
        
        [Header("Settings")]
        public float mouseSensitivity;
        public bool invert;
        public bool lockOnPlay = true;

        //- LOCAL VARIABLES
        private bool cameraLocked;
        private float Inverted => (invert) ? 1 : -1;
        private Vector2 mouseInput;
        
        //> INITIALIZATION
        private void Awake()
        {
            mouse = Mouse.current;
            keyboard = Keyboard.current;
            
            view = transform;
            cameraLocked = lockOnPlay;
            if (lockOnPlay) Cursor.lockState = CursorLockMode.Locked;

            mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 1);
            camera.fieldOfView = PlayerPrefs.GetFloat("fov", 90);
            
            
        }

        private void Update()
        {
            #if UNITY_EDITOR
            if (Keyboard.current.enterKey.wasPressedThisFrame) ToggleLockCursor();
            #endif
            
            if (!cameraLocked) return;
            
            mouseInput.x = Mouse.current.delta.x.ReadValue();
            mouseInput.y = Mouse.current.delta.y.ReadValue() * Inverted;
            mouseInput *= mouseSensitivity * Time.deltaTime;

        }

        private void LateUpdate()
        {
            view.localRotation *= Quaternion.AngleAxis(mouseInput.y, Vector3.right);
            player.rotation *= Quaternion.AngleAxis(mouseInput.x, Vector3.up);
        }

        //> TOGGLE LOCK CURSOR TO WINDOW
        private void ToggleLockCursor()
        {
            cameraLocked = !cameraLocked;

            if (Cursor.lockState == CursorLockMode.None) 
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else 
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}