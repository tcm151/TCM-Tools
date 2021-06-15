using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace TCM
{
    [RequireComponent(typeof(Rigidbody))]
    public class Boid : MonoBehaviour
    {
        private Rigidbody rigidbody;
        private Transform transform;
        private Vector3 forward;

        public float speed = 2f;
        public float detectionRadius = 2f;
        public float turnSpeed = 15f * 57.2958f;

        //> ON AWAKE
        private void Awake() => Initialize();

        //> INITIALIZATION
        public void Initialize()
        {
            transform = GetComponent<Transform>();

            rigidbody = GetComponent<Rigidbody>();
            rigidbody.position = Random.insideUnitSphere * Random.value * 10f;
            rigidbody.useGravity = false;

            forward = Random.insideUnitSphere.normalized;
        }

        //> APPLY MOVEMENT LOGIC
        private void FixedUpdate()
        {
            Teleport();
            Move();
            Avoid();
            // Align();
            // Group();
        }

        //> STAY WITHIN PLAY AREA
        private void Teleport()
        {
            if (transform.position.magnitude > 10f) transform.position = -transform.position.normalized * 9.95f;
        }

        //> MOVE
        private void Move()
        {
            rigidbody.velocity = forward * speed;
            transform.forward = rigidbody.velocity;
        }

        //> AVOID OTHER BOIDS
        private void Avoid()
        {
            var colliders = Physics.OverlapSphere(rigidbody.position, detectionRadius);

            foreach (var collider in colliders)
            {
                Boid boid;
                if ((boid = collider.GetComponent<Boid>()) is null) continue;

                Vector3 directionToBoid = (transform.forward - boid.transform.position).normalized;
                // if ((Vector3.Dot(transform.forward, directionToBoid)) < 0f) continue;

                transform.forward = Vector3.RotateTowards(transform.forward, directionToBoid, turnSpeed, 0f);

            }
        }

        //> ALIGN WITH OTHER BOIDS
        private void Align() { }

        //> GROUP WITH OTHER BOIDS
        private void Group() { }

        private void OnDrawGizmosSelected()
        {
            throw new NotImplementedException();
        }
    }
}