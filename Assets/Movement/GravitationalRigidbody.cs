using System;
using UnityEngine;

namespace TCM.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class GravitationalRigidbody : MonoBehaviour
    {
        private float delay;
        new private Rigidbody rigidbody;
        [SerializeField] private bool canSleep = false;

        //> INITIALIZATION
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
        }

        //> CHECK IF OBJECT CAN SLEEP OR APPLY GRAVITY
        private void FixedUpdate()
        {
            if (canSleep)
            {
                if (rigidbody.IsSleeping()) return;

                if (rigidbody.velocity.sqrMagnitude < 0.0001f)
                {
                    delay += Time.deltaTime;
                    if (delay >= 1f) return;
                }
            }
            else delay = 0f;
            
            rigidbody.AddForce(Gravity.Down(rigidbody.position), ForceMode.Acceleration);
        }
    }
}