using System.Collections;
using System.Linq;
using FSR.DigitalTwin.Client.Features.Robotics.KinematicRobot;
using RosMessageTypes.Geometry;
using RosMessageTypes.Ur5eMoveit;
using UniRx;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.Controller
{
    /// <summary>
    /// A robot controller that uses the MoveIt service running in a ROS2 workspace for planning.
    /// </summary>
    public class RosMoveitPickAndPlaceController : RobotControllerComponent
    {

        // MoveIt variables
        [SerializeField] private int numRobotJoints = 6;
        [SerializeField] private float jointAssignmentWait = 0.1f;
        [SerializeField] private float poseAssignmentWait = 0.5f;

        [SerializeField] private string rosServiceName = "ur5e_moveit";
        public string RosServiceName { get => rosServiceName; set => rosServiceName = value; }

        [SerializeField] private string[] linkNames = { "world/base_link/shoulder_link", "/upper_arm_link", "/forearm_link", "/wrist_1_link", "/wrist_2_link", "/wrist_3_link" };
        public string[] LinkNames => linkNames;

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
        private MoverServiceResponse _plannedTrajectory = null;
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
            _ros.RegisterRosService<MoverServiceRequest, MoverServiceResponse>(rosServiceName);

            _jointArticulationBodies = new ArticulationBody[numRobotJoints];

            var linkName = string.Empty;
            for (var i = 0; i < numRobotJoints; i++)
            {
                linkName += linkNames[i];
                _jointArticulationBodies[i] = robot.transform.Find(linkName).GetComponent<ArticulationBody>();
            }
        }

        /// <summary>
        ///     Get the current values of the robot's joint angles.
        /// </summary>
        /// <returns>NiryoMoveitJoints</returns>
        private UR5eMoveitJointsMsg CurrentJointConfig()
        {
            var joints = new UR5eMoveitJointsMsg();
            for (var i = 0; i < numRobotJoints; i++)
            {
                joints.joints[i] = _jointArticulationBodies[i].jointPosition[0];
            }
            return joints;
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
        ///     Create a new MoverServiceRequest with the current values of the robot's joint angles,
        ///     the target cube's current position and rotation, and the targetPlacement position and rotation.
        ///     Call the MoverService using the ROSConnection and if a trajectory is successfully planned,
        ///     store the response in the controller's state variable.
        /// </summary>
        public override void Plan()
        {
            var request = new MoverServiceRequest();
            request.joints_input = CurrentJointConfig();
            // Pick Pose
            request.pick_pose = new PoseMsg
            {
                position = (target.transform.position - robot.transform.position + pickPoseOffset).To<FLU>(),
                // The hardcoded x/z angles assure that the gripper is always positioned above the target cube before grasping.
                orientation = (pickOrientation * Quaternion.Euler(0.0f, -target.transform.eulerAngles.y, 0.0f)).To<FLU>() // Quaternion.identity.To<FLU>() // Quaternion.Euler(90, m_Target.transform.eulerAngles.y, 0).To<FLU>()
            };
            // Place Pose
            request.place_pose = new PoseMsg
            {
                position = (targetPlacement.transform.position - robot.transform.position + pickPoseOffset).To<FLU>(),
                orientation = pickOrientation.To<FLU>()
            };
            _ros.SendServiceMessage<MoverServiceResponse>(rosServiceName, request, OnTrajectoryResponse);
        }

        private void OnTrajectoryResponse(MoverServiceResponse response)
        {
            _plannedTrajectory = response;
            _hasPlanned.Value = true;
        }

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
        ///     Execute the returned trajectories from the MoverService.
        ///     The expectation is that the MoverService will return four trajectory plans,
        ///     PreGrasp, Grasp, PickUp, and Place,
        ///     where each plan is an array of robot poses. A robot pose is the joint angle values
        ///     of the six robot joints.
        ///     Executing a single trajectory will iterate through every robot pose in the array while updating the
        ///     joint values on the robot.
        /// </summary>
        /// <param name="response"> MoverServiceResponse received from niryo_moveit mover service running in ROS</param>
        /// <returns></returns>
        IEnumerator ExecuteTrajectories(MoverServiceResponse response)
        {
            if (response.trajectories != null)
            {
                // First things first open gripper
                gripper.OpenGripper();

                // For every trajectory plan returned
                for (var poseIndex = 0; poseIndex < response.trajectories.Length; poseIndex++)
                {
                    // For every robot pose in trajectory plan
                    foreach (var t in response.trajectories[poseIndex].joint_trajectory.points)
                    {
                        var jointPositions = t.positions;
                        var result = jointPositions.Select(r => (float)r * Mathf.Rad2Deg).ToArray();

                        // Set the joint values for every joint
                        for (var joint = 0; joint < _jointArticulationBodies.Length; joint++)
                        {
                            var joint1XDrive = _jointArticulationBodies[joint].xDrive;
                            joint1XDrive.target = result[joint];
                            _jointArticulationBodies[joint].xDrive = joint1XDrive;
                        }

                        // Wait for robot to achieve pose for all joint assignments
                        yield return new WaitForSeconds(jointAssignmentWait);
                    }

                    // Close the gripper if completed executing the trajectory for the Grasp pose
                    if (poseIndex == (int)Poses.Grasp)
                    {
                        gripper.CloseGripper();
                    }

                    // Wait for the robot to achieve the final pose from joint assignment
                    yield return new WaitForSeconds(poseAssignmentWait);
                }

                // All trajectories have been executed, open the gripper to place the target cube
                gripper.OpenGripper();

                // Finally free running action
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
    }
}