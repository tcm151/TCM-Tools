using UnityEngine;

namespace TCM.HexGrid
{
    public class StrategyView : MonoBehaviour
    {
        private Transform swivel, stick;

        private float zoom = 1f, rotationAngle;

        public float zoomSensitivity = 0.25f;
        public float minMoveSpeed, maxMoveSpeed;
        public float rotationSpeed;
        public float stickMinZoom, stickMaxZoom, swivelMinZoom, swivelMaxZoom;

        private void Awake()
        {
            swivel = transform.GetChild(0);
            stick = swivel.GetChild(0);
        }

        private void Update()
        {
            float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
            if (zoomDelta != 0f) AdjustZoom(zoomDelta);

            float rotationDelta = Input.GetAxis("Rotation");
            if (rotationDelta != 0) AdjustRotation(rotationDelta);

            float xDelta = Input.GetAxis("Horizontal");
            float zDelta = Input.GetAxis("Vertical");
            if (xDelta != 0 || zDelta != 0) AdjustPosition(xDelta, zDelta);
        }

        private void AdjustZoom(float delta)
        {
            zoom = Mathf.Clamp01(zoom + delta);

            float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
            stick.localPosition = new Vector3(0f, 0f, distance);

            float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
            swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }

        private void AdjustPosition(float x, float z)
        {
            Vector3 direction = transform.localRotation * new Vector3(x, 0, z).normalized;
            float damping = Mathf.Max(Mathf.Abs(x), Mathf.Abs(z));
            float distance = Mathf.Lerp(maxMoveSpeed, minMoveSpeed, zoom) * damping * Time.deltaTime;

            Vector3 position = transform.localPosition;
            position += direction * distance;
            transform.localPosition = position;
        }

        private void AdjustRotation(float rotation)
        {
            rotationAngle += rotation * rotationSpeed * Time.deltaTime;
            if (rotationAngle <   0) rotationAngle += 360;
            if (rotationAngle > 360) rotationAngle -= 360;
            transform.localRotation = Quaternion.Euler(0, rotationAngle, 0);

        }
        
    }
}