using System.Collections;
using System.Linq;
using FSR.DigitalTwin.Client.Features.Robotics.KinematicRobot;
using FSR.DigitalTwin.Client.Features.Robotics.ROS.Utils;
using RosMessageTypes.Geometry;
using RosMessageTypes.FsrMoveit;
using UniRx;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;
using System;

namespace FSR.DigitalTwin.Client.Features.Robotics.Controller
{
    /// <summary>
    /// A robot controller that uses the MoveIt service running in a ROS2 workspace for planning
    /// Pick-and-Place movement.
    /// </summary>
    public class RosMoveitPickAndPlaceController : RobotControllerComponent
    {

        // MoveIt variables
        [SerializeField] private int numRobotJoints = 6;
        [SerializeField] private float jointAssignmentWait = 0.1f;
        [SerializeField] private float poseAssignmentWait = 0.5f;
        [SerializeField] private float pickPoseOffsetZ = 0.066f;
        [SerializeField] private float placePoseOffsetZ = 0.0f;
        [SerializeField] private float maxVelocity = 0.5f;
        [SerializeField] private float maxAcceleration = 0.5f;
        [SerializeField] private string groupName = "ur_manipulator";
        [SerializeField] private string eeName = "tool0";
        [SerializeField] private string baseLinkName = "base";
        [SerializeField] private bool forceMoveItRequest = false;

        [SerializeField] private string rosServiceName = "fsr_moveit_pick_and_place_srv";
        public string RosServiceName { get => rosServiceName; set => rosServiceName = value; }

        [SerializeField] private string[] linkNames = { "world/base_link/shoulder_link", "/upper_arm_link", "/forearm_link", "/wrist_1_link", "/wrist_2_link", "/wrist_3_link" };
        public string[] LinkNames => linkNames;
        [SerializeField] private string[] rosJointNames = { "shoulder_pan_joint", "shoulder_lift_joint", "elbow_joint", "wrist_1_joint", "wrist_2_joint", "wrist_3_joint" };
        public string[] RosJointNames => rosJointNames;

        [SerializeField] private GameObject robot;
        [SerializeField] private GameObject target;
        public GameObject Target { get => target; set => target = value; }
        [SerializeField] private GameObject targetPlacement;
        public GameObject TargetPlacement { get => targetPlacement; set => targetPlacement = value; }

        [SerializeField] private Quaternion pickOrientation = Quaternion.Euler(new Vector3(-180, 0, 0));
        [SerializeField] private Vector3 pickPoseOffset = Vector3.up * 0.2f;

        // Controller interface
        public override GameObject Robot { get => robot; }
        public override ReadOnlyReactiveProperty<bool> HasPlanned => _hasPlanned.ToReadOnlyReactiveProperty();
        public override ReadOnlyReactiveProperty<bool> IsValid => _isValid.ToReadOnlyReactiveProperty();
        public override ReadOnlyReactiveProperty<bool> IsInterrupted => _isInterrupted.ToReadOnlyReactiveProperty();
        public override ReadOnlyReactiveProperty<bool> IsRunning => _isRunning.ToReadOnlyReactiveProperty();

        private ReactiveProperty<bool> _hasPlanned = new(false);
        private ReactiveProperty<bool> _isValid = new(false);
        private ReactiveProperty<bool> _isInterrupted = new(false);
        private ReactiveProperty<bool> _isRunning = new(false);

        // Internal state
        private PickAndPlaceServiceResponse _plannedTrajectory = null;
        private ArticulationBody[] _jointArticulationBodies;
        private Coroutine _runningAction;

        // Base EE interface
        [SerializeField] private GripperBase gripper;
        public GripperBase Gripper { get => gripper; set => gripper = value; }

        // ROS Connector
        private ROSConnection _ros;

        /// <summary>
        ///     Find all robot joints in Awake() and add them to the jointArticulationBodies array.
        ///     Find left and right finger joints and assign them to their respective articulation body objects.
        /// </summary>
        protected override void OnInitComponent()
        {
            // Get ROS connection static instance
            _ros = ROSConnection.GetOrCreateInstance();
            _ros.RegisterRosService<PickAndPlaceServiceRequest, PickAndPlaceServiceResponse>(rosServiceName);
            _jointArticulationBodies = new ArticulationBody[numRobotJoints];
            var linkName = string.Empty;
            for (var i = 0; i < numRobotJoints; i++)
            {
                linkName += linkNames[i];
                _jointArticulationBodies[i] = robot.transform.Find(linkName).GetComponent<ArticulationBody>();
            }
        }

        public override void ForceInterrupt()
        {
            Interrupt();
        }

        public override bool Interrupt()
        {
            if (_runningAction != null)
            {
                StopCoroutine(_runningAction);
                _isInterrupted.Value = true;
            }
            return _isInterrupted.Value;
        }

        /// <summary>
        ///     Create a new PickAndPlaceServiceRequest with the current values of the robot's joint angles,
        ///     the target cube's current position and rotation, and the targetPlacement position and rotation.
        ///     Call the PickAndPlaceService using the ROSConnection and if a trajectory is successfully planned,
        ///     store the response in the controller's state variable.
        /// </summary>
        public override void Plan()
        {
            PickAndPlaceServiceRequest request = new()
            {
                joints_input = GetCurrentJointConfig(),
                group = GetMoveitGroupConfig(),
                pars = GetPickAndPlaceParameters()
            };
            string filename = GetTrajectoryFilePath(gameObject, target, targetPlacement);
            if (!forceMoveItRequest && TrajectoryHelper.IsAvaliable(filename, out PickAndPlaceServiceResponse response))
            {
                Debug.Log("response successfully loaded");
                _plannedTrajectory = response;
                _hasPlanned.Value = true;
            }
            else {
                Debug.Log("Request sent to server");
                _ros.SendServiceMessage<PickAndPlaceServiceResponse>(rosServiceName, request, OnTrajectoryResponse);
            }
        }

        private void OnTrajectoryResponse(PickAndPlaceServiceResponse response)
        {
            _plannedTrajectory = response;
            _hasPlanned.Value = true;
            string filename = GetTrajectoryFilePath(gameObject, target, targetPlacement);
            TrajectoryHelper.Save(filename, response.ToResponseData());
        }

        private static string GetTrajectoryFilePath(GameObject robot, GameObject from, GameObject to) 
            => $"ros.traj.{robot.name}${from.name}_to_{to.name}.moveit";

        public override void RunPlan()
        {
            _isInterrupted.Value = false;
            if (!HasPlanned.Value || !IsValid.Value || IsRunning.Value)
            {
                Debug.LogError("Failed to run planned trajectory!");
                return;
            }
            _runningAction = StartCoroutine(ExecuteTrajectories(_plannedTrajectory));
        }

        public override bool ValidatePlan()
        {
            _isValid.Value = _plannedTrajectory.trajectories.Length > 0;
            return _isValid.Value;
        }

        public void PickAndPlace()
        {
            if (!HasPlanned.Value) 
                Plan();
            HasPlanned
                .Where(x => x)
                .First()
                .Subscribe(_ =>
                {
                    if (ValidatePlan())
                    {
                        RunPlan();
                    }
                })
                .AddTo(this);
        }
        
        /// <summary>
        ///     Execute the returned trajectories from the PickAndPlaceService.
        ///     The expectation is that the PickAndPlaceService will return four trajectory plans,
        ///     PreGrasp, Grasp, PickUp, and Place,
        ///     where each plan is an array of robot poses. A robot pose is the joint angle values
        ///     of the six robot joints.
        ///     Executing a single trajectory will iterate through every robot pose in the array while updating the
        ///     joint values on the robot.
        /// </summary>
        /// <param name="response"> PickAndPlaceServiceResponse received from niryo_moveit mover service running in ROS</param>
        /// <returns></returns>
        IEnumerator ExecuteTrajectories(PickAndPlaceServiceResponse response)
        {
            if (response.trajectories != null)
            {
                gripper.OpenGripper();
                for (var poseIndex = 0; poseIndex < response.trajectories.Length; poseIndex++)
                {
                    foreach (var t in response.trajectories[poseIndex].joint_trajectory.points)
                    {
                        var jointPositions = t.positions;
                        var result = jointPositions.Select(r => (float)r * Mathf.Rad2Deg).ToArray();
                        for (var joint = 0; joint < _jointArticulationBodies.Length; joint++)
                        {
                            var joint1XDrive = _jointArticulationBodies[joint].xDrive;
                            joint1XDrive.target = result[joint];
                            _jointArticulationBodies[joint].xDrive = joint1XDrive;
                        }
                        yield return new WaitForSeconds(jointAssignmentWait);
                    }
                    if (poseIndex == (int)Poses.Grasp)
                    {
                        gripper.CloseGripper();
                    }
                    yield return new WaitForSeconds(poseAssignmentWait);
                }
                gripper.OpenGripper();
                _runningAction = null;
            }
        }

        enum Poses
        {
            PreGrasp,
            Grasp,
            PickUp,
            Place
        }

        // ROS Messages

        private MoveitJointsMsg GetCurrentJointConfig()
        {
            var joints = new MoveitJointsMsg { joint_names = RosJointNames };
            for (var i = 0; i < numRobotJoints; i++)
            {
                joints.joints[i] = _jointArticulationBodies[i].jointPosition[0];
            }
            return joints;
        }
        private PickAndPlaceInputMsg GetPickAndPlaceParameters() => new()
            {
                pick_pose = new PoseMsg
                {
                    position = (target.transform.position - robot.transform.position + pickPoseOffset).To<FLU>(),
                    orientation = (pickOrientation * Quaternion.Euler(0.0f, -target.transform.eulerAngles.y, 0.0f)).To<FLU>()
                },
                place_pose = new PoseMsg
                {
                    position = (targetPlacement.transform.position - robot.transform.position + pickPoseOffset).To<FLU>(),
                    orientation = pickOrientation.To<FLU>()
                },
                pick_pose_z = pickPoseOffsetZ,
                place_pose_z = placePoseOffsetZ,
                max_velocity = maxVelocity,
                max_acceleration = maxAcceleration,
            };
        private MoveitGroupMsg GetMoveitGroupConfig() => new()
            {
                group_name = groupName,
                end_effector_name = eeName,
                base_link_name = baseLinkName
            };
    } // RosMoveitPickAndPlaceController
} // namespace FSR.DigitalTwin.Client.Features.Robotics.Controller