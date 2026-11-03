using UnityEngine;
using CityLifeIndia.Player;
using CityLifeIndia.Core;
using CityLifeIndia.Camera;

namespace CityLifeIndia.Vehicles
{
    [RequireComponent(typeof(Rigidbody))]
    public class BikeController : MonoBehaviour
    {
        [Header("Motorcycle Specs")]
        [SerializeField] private string bikeName = "Rajputana 350";
        [SerializeField] private float accelerationPower = 2000f;
        [SerializeField] private float maxSpeedKmH = 110f;
        [SerializeField] private float turnSensitivity = 45f;
        [SerializeField] private float maxLeanAngle = 28f;
        [SerializeField] private float brakeForce = 2500f;

        [Header("Visual Mesh & Transform")]
        [SerializeField] private Transform bikeChassis;
        [SerializeField] private Transform frontWheel;
        [SerializeField] private Transform rearWheel;
        [SerializeField] private Transform handleBar;
        [SerializeField] private Transform riderSeat;
        [SerializeField] private Transform exitPoint;

        [Header("Audio")]
        [SerializeField] private AudioSource engineThumpAudio;

        private Rigidbody rb;
        private PlayerController activeRider;
        private bool isOccupied = false;
        private float currentLean = 0f;
        private float speedKmH = 0f;

        public bool IsOccupied => isOccupied;
        public float SpeedKmH => speedKmH;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = new Vector3(0, -0.3f, 0);
        }

        private void Update()
        {
            speedKmH = rb.velocity.magnitude * 3.6f;

            if (isOccupied)
            {
                HandleInputs();
            }

            AnimateWheelsAndLean();
            UpdateAudio();
        }

        private void FixedUpdate()
        {
            if (isOccupied)
            {
                DriveBike();
            }
        }

        private void HandleInputs()
        {
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E))
            {
                ExitBike();
            }
        }

        private void DriveBike()
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");
            bool brake = Input.GetKey(KeyCode.Space);

            // Forward / Backward thrust
            if (!brake && speedKmH < maxSpeedKmH)
            {
                Vector3 force = transform.forward * v * accelerationPower * Time.fixedDeltaTime;
                rb.AddForce(force, ForceMode.Acceleration);
            }

            // Steering
            if (speedKmH > 1.5f)
            {
                float turnDir = Mathf.Sign(Vector3.Dot(rb.velocity, transform.forward));
                float yaw = h * turnSensitivity * turnDir * Time.fixedDeltaTime;
                Quaternion turnRotation = Quaternion.Euler(0f, yaw, 0f);
                rb.MoveRotation(rb.rotation * turnRotation);
            }

            // Braking
            if (brake)
            {
                rb.AddForce(-rb.velocity.normalized * brakeForce * Time.fixedDeltaTime, ForceMode.Acceleration);
            }
        }

        private void AnimateWheelsAndLean()
        {
            float h = isOccupied ? Input.GetAxis("Horizontal") : 0f;

            // Lean body when turning
            float targetLean = -h * maxLeanAngle * Mathf.Clamp01(speedKmH / 30f);
            currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * 6f);

            if (bikeChassis != null)
            {
                bikeChassis.localRotation = Quaternion.Euler(0f, 0f, currentLean);
            }

            if (handleBar != null)
            {
                handleBar.localRotation = Quaternion.Euler(0f, h * 25f, 0f);
            }

            // Wheel rotation spin
            float spinAngle = (speedKmH / 3.6f) / 0.35f * Mathf.Rad2Deg * Time.deltaTime;
            if (frontWheel != null) frontWheel.Rotate(Vector3.right, spinAngle);
            if (rearWheel != null) rearWheel.Rotate(Vector3.right, spinAngle);
        }

        private void UpdateAudio()
        {
            if (engineThumpAudio != null)
            {
                if (isOccupied)
                {
                    if (!engineThumpAudio.isPlaying) engineThumpAudio.Play();
                    engineThumpAudio.pitch = Mathf.Lerp(0.7f, 2.0f, speedKmH / maxSpeedKmH);
                }
                else
                {
                    if (engineThumpAudio.isPlaying) engineThumpAudio.Stop();
                }
            }
        }

        public void MountBike(PlayerController player)
        {
            activeRider = player;
            isOccupied = true;

            var camCtrl = FindFirstObjectByType<CameraController>();
            if (camCtrl != null)
            {
                camCtrl.SetTarget(transform);
            }

            GameManager.Instance?.SetState(GameState.InVehicle);
        }

        public void ExitBike()
        {
            if (!isOccupied || activeRider == null) return;

            Vector3 outPos = exitPoint != null ? exitPoint.position : transform.position + (transform.right * -1.5f);
            activeRider.ExitVehicle(outPos);

            var camCtrl = FindFirstObjectByType<CameraController>();
            if (camCtrl != null)
            {
                camCtrl.SetTarget(activeRider.transform);
            }

            isOccupied = false;
            activeRider = null;
            GameManager.Instance?.SetState(GameState.Gameplay);
        }
    }
}
