using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace TCM.Planets
{
    public class God : MonoBehaviour
    {
        new private Camera camera;

        private void Awake()
        {
            camera = Camera.main;
        }

        private void Update()
        {
            if (Physics.Raycast(camera.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit))
            {
                
            }
        }
    }
}