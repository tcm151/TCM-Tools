using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyMesh
{
    public class MeshEditorCamera : MonoBehaviour
    {
        /// LOCAL VARIABLES
        private Keyboard keyboard = Keyboard.current;
        private Mouse mouse = Mouse.current;
        private Transform origin;
        new private Transform camera;
        
        /// MOUSE INPUT
        private float mouseX, mouseY;
        private Vector2 mouseMovement;
        [Range(0.1f, 5f)] public float sensitivity = 1f;

        /// STATE CHECKS
        public bool Orbiting => mouse.middleButton.isPressed;
        public bool IsMouseOverGameView =>
        !(
            0 > mouse.position.x.ReadValue() ||
            0 > mouse.position.y.ReadValue() ||
            Screen.width < mouse.position.x.ReadValue()||
            Screen.height < mouse.position.x.ReadValue()
        );

        private void OnEnable()
        {
            camera = Camera.main.transform;
            origin = transform.GetChild(0);

            Mouse mouse = Mouse.current;
            Keyboard keyboard = Keyboard.current;
        }

        private void Update()
        {
            if (!IsMouseOverGameView) return;

            float zoomDelta = mouse.scroll.y.ReadValue();

            

            if (Orbiting)
            {
                mouseMovement = mouse.delta.ReadValue() * sensitivity;

                transform.rotation *= Quaternion.AngleAxis(mouseMovement.x, UnityEngine.Vector3.up);
                transform.rotation *= Quaternion.AngleAxis(mouseMovement.y, transform.right);

                origin.rotation = Quaternion.identity;
            }
        }

        // private void LateUpdate()
        // {
        //     Vector3 lookDirection = transform.position - camera.position;
        //     camera.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        // }

        // private void OnDrawGizmos()
        // {
        //     if (Application.isPlaying)
        //     {
        //         Gizmos.color = Color.white;
        //         Gizmos.DrawCube(transform.position, Vector3.one * 0.1f);
        //     }
        // }

    }
}
