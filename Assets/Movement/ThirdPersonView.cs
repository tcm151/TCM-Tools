
using UnityEngine;
using UnityEngine.InputSystem;


namespace TCM
{
    [RequireComponent(typeof(Camera))]
    public class ThirdPersonView : MonoBehaviour
    {
        [SerializeField] private Transform focus;
        [SerializeField, Min(0f)] private float focusRadius = 1f;
        [SerializeField, Range(1f, 20f)] private float focusDistance = 5f;
        [SerializeField, Range(0f, 1f)] private float focusCentering = 0.5f;
        [SerializeField, Range(1f, 360f)] private float rotationSpeed = 90f;
        [SerializeField, Range(-89f, 89f)] private float minVerticalAngle = -30f, maxVerticalAngle = 60f;
        [SerializeField, Min(0f)] private float alignDelay = 5f;
        [SerializeField, Range(0f, 90f)] private float alignSmoothRange = 45f;


        private Vector3 focusPosition, previousFocusPosition;
        private Vector2 orbitAngles = new Vector2(45f, 0f);
        private float lastManualRotationTime;

        private void Awake()
        {
            focusPosition = focus.position;
            transform.localRotation = Quaternion.Euler(orbitAngles);
        }

        private void OnValidate()
        {
            if (maxVerticalAngle < minVerticalAngle) maxVerticalAngle = minVerticalAngle;
        }

        private void LateUpdate()
        {
            UpdateFocusPoint();
            Quaternion lookRotation;
            if (ManualRotation() || AutomaticRotation())
            {
                ConstrainAngles();
                lookRotation = Quaternion.Euler(orbitAngles);
            }
            else lookRotation = transform.localRotation;
            
            Vector3 lookDirection = lookRotation * Vector3.forward;
            Vector3 lookPosition = focusPosition - lookDirection * focusDistance;

            if (Physics.Raycast(focusPosition, -lookDirection, out RaycastHit hit, focusDistance))
            {
                lookPosition = focusPosition - lookDirection * hit.distance;
            }
            
            
            transform.SetPositionAndRotation(lookPosition, lookRotation);
        }

        bool ManualRotation()
        {
            Vector2 input = Mouse.current.delta.ReadValue();
            const float e = 0.001f;
            if (input.x < -e || input.x > e || input.y < -e || input.y > e)
            {
                orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
                lastManualRotationTime = Time.unscaledTime;
                return true;
            }

            return false;
        }

        bool AutomaticRotation()
        {
            if (Time.unscaledTime - lastManualRotationTime < alignDelay) return false;

            Vector2 movement = new Vector2(focusPosition.x - previousFocusPosition.x, focusPosition.z - previousFocusPosition.z);
            float movementDeltaSquared = movement.sqrMagnitude;
            if (movementDeltaSquared < 0.000001f) return false;

            float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSquared));
            float absoluteDelta = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
            float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSquared);
            if (absoluteDelta < alignSmoothRange) rotationChange *= absoluteDelta / alignSmoothRange;
            else if (180f - absoluteDelta < alignSmoothRange) rotationChange *= (180f - absoluteDelta) / alignSmoothRange;
            orbitAngles.y = Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
            return true;
        }

        private static float GetAngle(Vector3 direction)
        {
            float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
            return (direction.x < 0f) ? 360f - angle : angle;
        }

        void ConstrainAngles()
        {
            orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

            if (orbitAngles.y < 0f) orbitAngles.y += 360f;
            else if (orbitAngles.y >= 360f) orbitAngles.y -= 360f;
        }

        private void UpdateFocusPoint()
        {
            previousFocusPosition = focusPosition;
            Vector3 targetPosition = focus.position;
            if (focusRadius > 1)
            {
                float distance = Vector3.Distance(targetPosition, focusPosition);
                float t = 1f;
                if (distance > 0.01f && focusCentering > 0f)
                {
                    t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
                }
                if (distance > focusRadius)
                {
                    t = Mathf.Min(t, focusRadius / distance);
                }
            }
            else focusPosition = targetPosition;
        }
    }
}
