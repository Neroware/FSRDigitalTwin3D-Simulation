using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FSR.DigitalTwin.Client.Features.Player
{

    [RequireComponent(typeof(CharacterController))]
    public class SimpleFirstPersonController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float lookSpeed = 2f;
        public float jumpHeight = 1.5f;
        public float gravity = -9.81f;

        private CharacterController controller;
        private Vector2 moveInput;
        private Vector2 lookInput;
        private float verticalLookRotation = 0f;
        private Transform cameraTransform;

        private float verticalVelocity = 0f;
        private bool jumpPressed = false;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            cameraTransform = GetComponentInChildren<Camera>().transform;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                jumpPressed = true;
            }
        }

        private void Update()
        {
            // Bewegung
            Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;
            if (controller.isGrounded)
            {
                verticalVelocity = -2f;
                if (jumpPressed)
                {
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    jumpPressed = false;
                }
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            move.y = verticalVelocity;
            controller.Move(move * moveSpeed * Time.deltaTime);

            // Mausbewegung
            transform.Rotate(Vector3.up * lookInput.x * lookSpeed);

            verticalLookRotation -= lookInput.y * lookSpeed;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
            cameraTransform.localEulerAngles = Vector3.right * verticalLookRotation;
        }
    }

}