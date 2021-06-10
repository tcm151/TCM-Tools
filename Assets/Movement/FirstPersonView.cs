using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace TCM.Movement
{
    public class FirstPersonView : MonoBehaviour
    {
        public Transform player;
        public Camera cam;
        
        public bool cameraLocked, invert = false;
        public float mouseSensitivity;

        public float Inverted => (invert) ? 1 : -1;

        private Transform view;
        private Vector2 input;
        
        [Header("Options")]
        public bool lockOnPlay = true;

        private void Awake()
        {
            cameraLocked = lockOnPlay;
            if (lockOnPlay) Cursor.lockState = CursorLockMode.Locked;
            
            view = transform;
        }

        private void Update()
        {
            #if UNITY_EDITOR
            if (Keyboard.current.enterKey.wasPressedThisFrame) ToggleLockCursor();
            #endif
            
            if (!cameraLocked) return;
            
            float mouseX = Mouse.current.delta.x.ReadValue() * Time.deltaTime;
            float mouseY = Mouse.current.delta.y.ReadValue() * Time.deltaTime * Inverted;
            input = new Vector2(mouseX, mouseY) * mouseSensitivity;
            
        }

        private void LateUpdate()
        {
            view.localRotation *= Quaternion.AngleAxis(input.y, Vector3.right);
            player.rotation *= Quaternion.AngleAxis(input.x, Vector3.up);
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

        public void SetMouseSensitivity(float sens) => mouseSensitivity = sens;

        public void SetFOV(float fov) => cam.fieldOfView = fov;

        public void SetInvertMouse(bool truth) => invert = truth;

        public Vector3 CameraRelative(Vector3 v) => v = view.TransformDirection(v);
    }
}