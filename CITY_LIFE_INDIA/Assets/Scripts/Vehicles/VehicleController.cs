using System;
using UnityEngine;
using CityLifeIndia.Player;
using CityLifeIndia.Core;
using CityLifeIndia.Camera;

namespace CityLifeIndia.Vehicles
{
    public enum VehicleType
    {
        Car,
        Bike,
        SUV,
        Bus
    }

    [RequireComponent(typeof(Rigidbody))]
    public class VehicleController : MonoBehaviour
    {
        [Header("Vehicle Configuration")]
        [SerializeField] private VehicleType vehicleType = VehicleType.Car;
        [SerializeField] private string vehicleName = "Desi Cruiser";
        [SerializeField] private float maxMotorTorque = 1600f;
        [SerializeField] private float maxBrakeTorque = 3000f;
        [SerializeField] private float maxSteerAngle = 32f;
        [SerializeField] private float topSpeedKmH = 140f;

        [Header("Wheels Colliders (Car)")]
        [SerializeField] private WheelCollider frontLeftCollider;
        [SerializeField] private WheelCollider frontRightCollider;
        [SerializeField] private WheelCollider rearLeftCollider;
        [SerializeField] private WheelCollider rearRightCollider;

        [Header("Wheel Visual Meshes")]
        [SerializeField] private Transform frontLeftMesh;
        [SerializeField] private Transform frontRightMesh;
        [SerializeField] private Transform rearLeftMesh;
        [SerializeField] private Transform rearRightMesh;

        [Header("Vehicle Visuals & Lighting")]
        [SerializeField] private MeshRenderer bodyRenderer;
        [SerializeField] private GameObject headLightsObj;
        [SerializeField] private GameObject brakeLightsObj;

        [Header("Interaction Points")]
        [SerializeField] private Transform driverSeatPoint;
        [SerializeField] private Transform exitPoint;

        [Header("Audio")]
        [SerializeField] private AudioSource engineAudio;
        [SerializeField] private AudioSource hornAudio;

        private Rigidbody rb;
        private PlayerController activeDriver;
        private bool isOccupied = false;
        private float currentSpeedKmH = 0f;
        private bool headlightsOn = false;

        public bool IsOccupied => isOccupied;
        public float CurrentSpeedKmH => currentSpeedKmH;
        public VehicleType Type => vehicleType;
        public string VehicleName => vehicleName;

        public event Action<float> OnSpeedChanged;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = new Vector3(0, -0.4f, 0.2f); // Lower center of mass for rollover stability
        }

        private void Update()
        {
            CalculateSpeed();

            if (isOccupied)
            {
                HandleInputs();
            }

            UpdateWheelVisuals();
            UpdateAudio();
        }

        private void FixedUpdate()
        {
            if (isOccupied)
            {
                Drive();
            }
            else
            {
                ApplyHandbrake();
            }
        }

        private void CalculateSpeed()
        {
            currentSpeedKmH = rb.velocity.magnitude * 3.6f;
            OnSpeedChanged?.Invoke(currentSpeedKmH);

            var camCtrl = FindFirstObjectByType<CameraController>();
            if (camCtrl != null && isOccupied)
            {
                camCtrl.UpdateDrivingFOV(Mathf.Clamp01(currentSpeedKmH / topSpeedKmH));
            }
        }

        private void HandleInputs()
        {
            // Exit vehicle
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E))
            {
                ExitVehicle();
            }

            // Headlights toggle
            if (Input.GetKeyDown(KeyCode.L))
            {
                ToggleHeadlights();
            }

            // Horn
            if (Input.GetKeyDown(KeyCode.H))
            {
                PlayHorn();
            }
        }

        private void Drive()
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");
            bool isBraking = Input.GetKey(KeyCode.Space);

            // Steering
            float steer = h * maxSteerAngle;
            if (frontLeftCollider != null) frontLeftCollider.steerAngle = steer;
            if (frontRightCollider != null) frontRightCollider.steerAngle = steer;

            // Motor Torque
            float motor = 0f;
            if (currentSpeedKmH < topSpeedKmH)
            {
                motor = v * maxMotorTorque;
            }

            if (rearLeftCollider != null) rearLeftCollider.motorTorque = motor;
            if (rearRightCollider != null) rearRightCollider.motorTorque = motor;

            // Brake
            float brake = isBraking ? maxBrakeTorque : 0f;
            if (frontLeftCollider != null) frontLeftCollider.brakeTorque = brake;
            if (frontRightCollider != null) frontRightCollider.brakeTorque = brake;
            if (rearLeftCollider != null) rearLeftCollider.brakeTorque = brake;
            if (rearRightCollider != null) rearRightCollider.brakeTorque = brake;

            if (brakeLightsObj != null)
            {
                brakeLightsObj.SetActive(isBraking || (v < -0.1f && currentSpeedKmH > 2f));
            }
        }

        private void ApplyHandbrake()
        {
            if (rearLeftCollider != null) rearLeftCollider.brakeTorque = maxBrakeTorque;
            if (rearRightCollider != null) rearRightCollider.brakeTorque = maxBrakeTorque;
        }

        private void UpdateWheelVisuals()
        {
            UpdateSingleWheel(frontLeftCollider, frontLeftMesh);
            UpdateSingleWheel(frontRightCollider, frontRightMesh);
            UpdateSingleWheel(rearLeftCollider, rearLeftMesh);
            UpdateSingleWheel(rearRightCollider, rearRightMesh);
        }

        private void UpdateSingleWheel(WheelCollider collider, Transform mesh)
        {
            if (collider == null || mesh == null) return;
            collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
            mesh.position = pos;
            mesh.rotation = rot;
        }

        private void UpdateAudio()
        {
            if (engineAudio != null)
            {
                if (isOccupied)
                {
                    if (!engineAudio.isPlaying) engineAudio.Play();
                    float pitchMultiplier = Mathf.Lerp(0.8f, 2.2f, currentSpeedKmH / topSpeedKmH);
                    engineAudio.pitch = pitchMultiplier;
                }
                else
                {
                    if (engineAudio.isPlaying) engineAudio.Stop();
                }
            }
        }

        public void EnterVehicle(PlayerController player)
        {
            activeDriver = player;
            isOccupied = true;

            var camCtrl = FindFirstObjectByType<CameraController>();
            if (camCtrl != null)
            {
                camCtrl.SetTarget(transform);
            }

            GameManager.Instance?.SetState(GameState.InVehicle);
            Debug.Log($"[VehicleController] Player entered {vehicleName}");
        }

        public void ExitVehicle()
        {
            if (!isOccupied || activeDriver == null) return;

            Vector3 outPos = exitPoint != null ? exitPoint.position : transform.position + (transform.right * -2f);
            activeDriver.ExitVehicle(outPos);

            var camCtrl = FindFirstObjectByType<CameraController>();
            if (camCtrl != null)
            {
                camCtrl.SetTarget(activeDriver.transform);
            }

            isOccupied = false;
            activeDriver = null;

            ApplyHandbrake();
            GameManager.Instance?.SetState(GameState.Gameplay);
            Debug.Log($"[VehicleController] Player exited {vehicleName}");
        }

        public void ToggleHeadlights()
        {
            headlightsOn = !headlightsOn;
            if (headLightsObj != null) headLightsObj.SetActive(headlightsOn);
        }

        public void PlayHorn()
        {
            if (hornAudio != null) hornAudio.Play();
        }

        public void SetPaintColor(Color newColor)
        {
            if (bodyRenderer != null)
            {
                bodyRenderer.material.color = newColor;
            }
        }
    }
}
