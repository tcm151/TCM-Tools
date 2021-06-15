using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCM
{
    public class Sun : MonoBehaviour
    {
        [Range(7500, 15000)] public float radius = 7500f;
        [Range(0.1f, 15.0f)] public float rotationSpeed = 1f;

        public Moon moon;
        new private Transform transform;
        
        private void OnValidate() => moon.rotationSpeed = rotationSpeed;

        private void Awake()
        {
            transform = GetComponent<Transform>();
            transform.position = Vector3.forward * radius;
            transform.rotation = Quaternion.LookRotation(-transform.position, Vector3.up);
        }
        
        private void FixedUpdate()
        {
            transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * Time.deltaTime);
            
            //$ DO MORE STUFF
        }

        public void SetPosition(Vector3 targetPosition)
        {
            //? YET TO BE IMPLEMENTED
        }
    }
}
