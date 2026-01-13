using FSR.DigitalTwin.Client.Features.Player.Controls;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Player
{

    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private FirstPersonPlayerInputActions inputActions;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float lookSpeed = 2f;
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float gravity = -9.81f;

        private CharacterController controller;
        private float verticalLookRotation = 0f;
        private Transform cameraTransform;

        private Vector3 linearVelocity = Vector3.zero;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            cameraTransform = GetComponentInChildren<Camera>().transform;
            Cursor.lockState = CursorLockMode.Locked;

            inputActions.Camera
                .Where(_ => gameObject.activeSelf)
                .Where(_ => Cursor.lockState == CursorLockMode.Locked)
                .Subscribe(OnCameraMove)
                .AddTo(this);
            inputActions.Move
                .Subscribe(OnMove)
                .AddTo(this);
            inputActions.Jump
                .Where(x => controller.isGrounded)
                .Subscribe(_ => OnJump())
                .AddTo(this);
            inputActions.ControlSelect
                .Subscribe(_ => {
                    Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? 
                        CursorLockMode.None : CursorLockMode.Locked;
                })
                .AddTo(this);
        }

        private void OnJump()
        {
            linearVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        private void OnCameraMove(Vector2 movement)
        {
            transform.Rotate(Vector3.up * movement.x * lookSpeed);
            verticalLookRotation -= movement.y * lookSpeed;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
            cameraTransform.localEulerAngles = Vector3.right * verticalLookRotation;
        }

        private void OnMove(Vector2 movement)
        {
            Vector3 v = transform.forward * movement.y + transform.right * movement.x;
            linearVelocity.x = moveSpeed * v.x;
            linearVelocity.z = moveSpeed * v.z;
        }

        private void Update()
        {
            controller.Move(Time.deltaTime * linearVelocity);

            linearVelocity.y = controller.isGrounded ? 
                -0.1f : linearVelocity.y + gravity * Time.deltaTime;

            // Decelerate immediately
            linearVelocity.x = linearVelocity.z = 0.0f;
        }
    }

}