// =================================================================================================================================== //
// A robot controller using a Social Force Model (SFM) to create a trajectory
//
// This script is based on a project work by Robert Figl
// =================================================================================================================================== //

using System;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.Robotics.Interfaces;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.Controller.SFM
{
    public class SFMRobotController : MonoBehaviour, IRobotController
    {
        [Header("Robot Controller")]
        [SerializeField] private GameObject robot;
        [SerializeField] private Transform goal;
        [SerializeField] private bool autoRun = false;
        [SerializeField] private ArticulationBody agentArticulationBody;
        
        [Header("Wheel Articulation Bodies")]
        [SerializeField] private ArticulationBody frontRightWheel;
        [SerializeField] private ArticulationBody frontLeftWheel;
        [SerializeField] private ArticulationBody backRightWheel;
        [SerializeField] private ArticulationBody backLeftWheel;

        [Header("Social Force Model")]
        [SerializeField] private SocialForceModel sfm;

        [Header("Drive Settings")]
        [Tooltip("Maximum wheel speed")]
        [SerializeField] private float wheelSpeed = 300f;
        [Tooltip("Distance between the wheels")]
        [SerializeField] private float distanceBetweenWheels = 2;
        [Tooltip("Size of the wheels")]
        [SerializeField] private float sizeOfWheels = 1;
        [SerializeField] private float stiffness = 10.0f;
        [SerializeField] private float damping = 150.0f;
        [SerializeField] private float forceLimit = 1000.0f;

        [Header("Clamping Settings")]
        [SerializeField] private float forwardClampMin = -1.0f;
        [SerializeField] private float forwardClampMax = 1.0f;
        [SerializeField] private float turnClampMin = -20.0f;
        [SerializeField] private float turnClampMax = 20.0f;

        [Header("Force Settings")]
        [Tooltip("Magnitude scaling factor for input force.")]
        [SerializeField] private float forceMagnitude = 1.0f;

        [Header("Smoothing Settings")]
        [Tooltip("Smoothing factor applied to forces for stable movement.")]
        [SerializeField] private float smoothingFactor = 0.1f;

        private Vector3 _smoothedForce = Vector3.zero;
        private readonly PIDController _turnPID = new(5.0f, 0.01f, 1.0f);

        private ReactiveProperty<bool> _hasPlanned = new(false);
        private ReactiveProperty<bool> _isValid = new(false);
        private ReactiveProperty<bool> _isInterrupted = new(false);
        private ReactiveProperty<bool> _isRunning = new(false);

        public GameObject Robot => robot;
        public ReadOnlyReactiveProperty<bool> HasPlanned => _hasPlanned.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<bool> IsValid => _isValid.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<bool> IsInterrupted => _isInterrupted.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<bool> IsRunning => _isRunning.ToReadOnlyReactiveProperty();

        private void Start()
        {
            if (!autoRun)
                return;
            Plan();
            if (ValidatePlan())
                RunPlan();
        }

        public void ForceInterrupt() => Interrupt();

        public bool Interrupt()
        {
            _isInterrupted.Value = true;
            _isRunning.Value = false;
            return _isRunning.Value;
        }

        public void Plan()
        {
            sfm.Goal = goal;
            if (sfm.Obstacles.Count == 1)
            {
                Transform parent = sfm.Obstacles[0];
                sfm.Obstacles.Clear();
                foreach (Transform child in parent)
                {
                    sfm.Obstacles.Add(child);
                }
                Debug.Log($"Loaded {sfm.Obstacles.Count} child obstacles from parent {parent.name}");
            }
            _hasPlanned.Value = true;
        }

        public void RunPlan()
        {
            if (!_hasPlanned.Value || !_isValid.Value)
            {
                Debug.Log("Create and validate movement plan first!");
            }
            else
            {
                _isRunning.Value = true;
                _isInterrupted.Value = false;
            }
        }

        public bool ValidatePlan()
        {
            _isValid.Value = sfm.Goal != null && sfm != null;
            return _isValid.Value;
        }

        private void FixedUpdate()
        {
            if (!_isRunning.Value)
                return;
            if (sfm == null)
                throw new NullReferenceException(nameof(sfm));

            Vector3 rawForce = sfm.TotalForce;
            _smoothedForce = Vector3.Lerp(_smoothedForce, rawForce, smoothingFactor);
            Vector3 force = _smoothedForce;

            if (force.magnitude < 0.001f)
                return;

            float forceMagnitude = force.magnitude;
            Vector3 directionToGoal = force.normalized;
            Vector3 localDirection = agentArticulationBody.transform.InverseTransformDirection(directionToGoal);
            float forward = Mathf.Clamp(localDirection.z * forceMagnitude, forwardClampMin, forwardClampMax);
            float turn = _turnPID.NextValue(localDirection.x, Time.fixedDeltaTime);
            turn = Mathf.Clamp(turn, turnClampMin, turnClampMax);
            float leftVelocity = (forward - turn * (distanceBetweenWheels / 2)) / sizeOfWheels * wheelSpeed;
            float rightVelocity = (forward + turn * (distanceBetweenWheels / 2)) / sizeOfWheels * wheelSpeed;

            ApplyVelocity(frontLeftWheel, leftVelocity);
            ApplyVelocity(backLeftWheel, leftVelocity);
            ApplyVelocity(frontRightWheel, rightVelocity);
            ApplyVelocity(backRightWheel, rightVelocity);

            // Debug.Log($"[SFM] F: {force}, Forward: {forward}, Turn: {turn}, L: {leftVelocity}, R: {rightVelocity}");
        }

        void ApplyVelocity(ArticulationBody wheel, float velocity)
        {
            var drive = wheel.xDrive;
            drive.targetVelocity = velocity;
            drive.stiffness = stiffness;
            drive.damping = damping;
            drive.forceLimit = forceLimit;
            wheel.xDrive = drive;
        }
    }
}