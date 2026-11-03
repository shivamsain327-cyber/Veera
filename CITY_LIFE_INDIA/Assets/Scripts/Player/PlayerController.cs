using System;
using UnityEngine;
using CityLifeIndia.Vehicles;

namespace CityLifeIndia.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 4.5f;
        [SerializeField] private float runSpeed = 8.5f;
        [SerializeField] private float rotationSpeed = 12f;
        [SerializeField] private float jumpHeight = 1.8f;
        [SerializeField] private float gravity = -20f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private float groundDistance = 0.3f;
        [SerializeField] private LayerMask groundMask;

        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 2.5f;
        [SerializeField] private LayerMask vehicleLayer;

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Transform cameraTransform;

        private CharacterController controller;
        private Vector3 velocity;
        private bool isGrounded;
        private bool isRunning;
        private VehicleController currentNearbyVehicle;
        private bool isInVehicle;

        public bool IsInVehicle => isInVehicle;
        public VehicleController CurrentNearbyVehicle => currentNearbyVehicle;

        public event Action<VehicleController> OnNearVehicleChanged;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (cameraTransform == null && UnityEngine.Camera.main != null)
            {
                cameraTransform = UnityEngine.Camera.main.transform;
            }
        }

        private void Update()
        {
            if (isInVehicle) return;

            CheckGround();
            HandleMovement();
            CheckNearbyVehicles();
            HandleInputs();
        }

        private void CheckGround()
        {
            if (groundCheckPoint != null)
            {
                isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundDistance, groundMask);
            }
            else
            {
                isGrounded = controller.isGrounded;
            }

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Keep grounded stick
            }
        }

        private void HandleMovement()
        {
            // Input from Keyboard or Touch Virtual Joystick
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            if (direction.magnitude >= 0.1f)
            {
                // Align movement direction with Camera orientation
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                float currentSpeed = isRunning ? runSpeed : walkSpeed;

                controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

                if (animator != null)
                {
                    animator.SetFloat("Speed", isRunning ? 2f : 1f, 0.15f, Time.deltaTime);
                }
            }
            else
            {
                if (animator != null)
                {
                    animator.SetFloat("Speed", 0f, 0.15f, Time.deltaTime);
                }
            }

            // Apply Gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        private void HandleInputs()
        {
            // Sprint Toggle
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                SetRunning(!isRunning);
            }

            // Jump
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                Jump();
            }

            // Enter Vehicle
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E))
            {
                TryEnterVehicle();
            }
        }

        public void SetRunning(bool run)
        {
            isRunning = run;
        }

        public void Jump()
        {
            if (isGrounded && !isInVehicle)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                }
            }
        }

        private void CheckNearbyVehicles()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRadius, vehicleLayer);
            VehicleController foundVehicle = null;

            foreach (var hit in hitColliders)
            {
                var vehicle = hit.GetComponentInParent<VehicleController>();
                if (vehicle != null && !vehicle.IsOccupied)
                {
                    foundVehicle = vehicle;
                    break;
                }
            }

            if (foundVehicle != currentNearbyVehicle)
            {
                currentNearbyVehicle = foundVehicle;
                OnNearVehicleChanged?.Invoke(currentNearbyVehicle);
            }
        }

        public bool TryEnterVehicle()
        {
            if (currentNearbyVehicle != null && !currentNearbyVehicle.IsOccupied)
            {
                isInVehicle = true;
                controller.enabled = false;
                gameObject.SetActive(false);
                currentNearbyVehicle.EnterVehicle(this);
                return true;
            }
            return false;
        }

        public void ExitVehicle(Vector3 exitPosition)
        {
            transform.position = exitPosition;
            gameObject.SetActive(true);
            controller.enabled = true;
            isInVehicle = false;
            velocity = Vector3.zero;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);

            if (groundCheckPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheckPoint.position, groundDistance);
            }
        }
    }
}
