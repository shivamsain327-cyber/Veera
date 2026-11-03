using UnityEngine;

namespace CityLifeIndia.Camera
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target Tracking")]
        [SerializeField] private Transform currentTarget;
        [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.8f, 0f);

        [Header("Distance & Angles")]
        [SerializeField] private float defaultDistance = 5.0f;
        [SerializeField] private float minDistance = 1.5f;
        [SerializeField] private float maxDistance = 9.0f;
        [SerializeField] private float mouseSensitivity = 3.0f;
        [SerializeField] private float touchSensitivity = 0.2f;

        [Header("Pitch Limits")]
        [SerializeField] private float minPitch = -15.0f;
        [SerializeField] private float maxPitch = 60.0f;

        [Header("Collision & Smoothing")]
        [SerializeField] private float smoothSpeed = 10f;
        [SerializeField] private LayerMask collisionLayers;
        [SerializeField] private float collisionOffset = 0.25f;

        [Header("Dynamic Driving FOV")]
        [SerializeField] private float normalFOV = 60f;
        [SerializeField] private float highSpeedFOV = 75f;

        private float currentYaw = 0f;
        private float currentPitch = 15f;
        private float currentDistance;
        private UnityEngine.Camera cam;
        private bool isFirstPerson = false;

        private void Awake()
        {
            cam = GetComponent<UnityEngine.Camera>();
            currentDistance = defaultDistance;
            if (cam != null) cam.fieldOfView = normalFOV;
        }

        public void SetTarget(Transform newTarget)
        {
            currentTarget = newTarget;
        }

        public void ToggleFirstPerson()
        {
            isFirstPerson = !isFirstPerson;
            currentDistance = isFirstPerson ? 0.05f : defaultDistance;
        }

        private void LateUpdate()
        {
            if (currentTarget == null) return;

            HandleInput();
            CalculatePosition();
        }

        private void HandleInput()
        {
            // Touch Swipe input on Mobile
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                // If touch is on right half of screen (camera look area as defined in GDD Section 4)
                if (touch.position.x > Screen.width * 0.45f && touch.phase == TouchPhase.Moved)
                {
                    currentYaw += touch.deltaPosition.x * touchSensitivity;
                    currentPitch -= touch.deltaPosition.y * touchSensitivity;
                }
            }
            else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                currentYaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                currentPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            }

            // Keyboard orbit alternative (Arrow keys or IJKL)
            if (Input.GetKey(KeyCode.RightArrow)) currentYaw += 75f * Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftArrow)) currentYaw -= 75f * Time.deltaTime;
            if (Input.GetKey(KeyCode.UpArrow)) currentPitch += 50f * Time.deltaTime;
            if (Input.GetKey(KeyCode.DownArrow)) currentPitch -= 50f * Time.deltaTime;

            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

            // Toggle camera mode with 'C'
            if (Input.GetKeyDown(KeyCode.C))
            {
                ToggleFirstPerson();
            }
        }

        private void CalculatePosition()
        {
            Vector3 targetPivot = currentTarget.position + targetOffset;
            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

            // Calculate desired position
            Vector3 desiredPosition = targetPivot - (rotation * Vector3.forward * currentDistance);

            // Obstacle Raycast check to prevent wall clipping
            if (!isFirstPerson)
            {
                RaycastHit hit;
                Vector3 rayDir = (desiredPosition - targetPivot).normalized;
                float rayDist = currentDistance;

                if (Physics.Raycast(targetPivot, rayDir, out hit, rayDist, collisionLayers))
                {
                    desiredPosition = hit.point + (hit.normal * collisionOffset);
                }
            }

            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
            transform.LookAt(targetPivot);
        }

        public void UpdateDrivingFOV(float speedNormalized)
        {
            if (cam != null)
            {
                float targetFOV = Mathf.Lerp(normalFOV, highSpeedFOV, speedNormalized);
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * 3f);
            }
        }
    }
}
