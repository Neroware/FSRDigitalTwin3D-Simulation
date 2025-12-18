using System;
using System.Collections;
using System.Linq;
using FSR.DigitalTwin.Client.Features.Robotics.ROS.Utils;
using RosMessageTypes.FsrMoveit;
using RosMessageTypes.Geometry;
using UniRx;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.Controller
{
    public class RosMoveitController : RobotControllerComponent
    {
        [SerializeField] private int numRobotJoints = 6;
        [SerializeField] private float jointAssignmentWait = 0.1f;
        [SerializeField] private float poseAssignmentWait = 0.5f;
        [SerializeField] private float maxVelocity = 0.5f;
        [SerializeField] private float maxAcceleration = 0.5f;
        [SerializeField] private string groupName = "ur_manipulator";
        // [SerializeField] private string eeName = "tool0";
        [SerializeField] private string baseLinkName = "base";
        [SerializeField] private bool forceMoveItRequest = false;
        [SerializeField] private string rosServiceName = "fsr_moveit_move_to_srv";
        public string RosServiceName { get => rosServiceName; set => rosServiceName = value; }

        [SerializeField] private string[] linkNames = { "world/base_link/shoulder_link", "/upper_arm_link", "/forearm_link", "/wrist_1_link", "/wrist_2_link", "/wrist_3_link" };
        public string[] LinkNames => linkNames;
        [SerializeField] private string[] rosJointNames = { "shoulder_pan_joint", "shoulder_lift_joint", "elbow_joint", "wrist_1_joint", "wrist_2_joint", "wrist_3_joint" };
        public string[] RosJointNames => rosJointNames;

        [SerializeField] private GameObject robot;
        [SerializeField] private Vector3 target;
        [SerializeField] private Vector3 targetOffset;
        public Vector3 Target { get => target; set => target = value; }

        [SerializeField] private Quaternion targetOrientation = Quaternion.Euler(new Vector3(-180, 0, 0));

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
        private ArticulationBody[] _jointArticulationBodies;
        private Coroutine _runningAction;
        private MoveToServiceResponse _plannedTrajectory = null;
        private string trajectoryName = "my_trajectory";

        public string TrajectoryName { set => trajectoryName = value; }

        // ROS Connector
        private ROSConnection _ros;

        protected override void OnInitComponent()
        {
            _ros = ROSConnection.GetOrCreateInstance();
            _ros.RegisterRosService<MoveToServiceRequest, MoveToServiceResponse>(rosServiceName);
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
        public override void Plan()
        {
            MoveToServiceRequest request = new()
            {
                joints_input = GetCurrentJointConfig(),
                group = GetMoveitGroupConfig(),
                pars = GetMoveParameters()
            };
            string filename = GetTrajectoryFilePath(trajectoryName, robot);
            if (!forceMoveItRequest && TrajectoryHelper.IsAvaliable(filename, out MoveToServiceResponse response))
            {
                Debug.Log("response successfully loaded");
                _plannedTrajectory = response;
                _hasPlanned.Value = true;
            }
            else {
                Debug.Log("Request sent to server");
                _ros.SendServiceMessage<MoveToServiceResponse>(rosServiceName, request, OnTrajectoryResponse);
            }
        }
        private void OnTrajectoryResponse(MoveToServiceResponse response)
        {
            _plannedTrajectory = response;
            _hasPlanned.Value = true;
            string filename = GetTrajectoryFilePath(trajectoryName, robot);
            TrajectoryHelper.Save(filename, response.ToResponseData());
        }
        private string GetTrajectoryFilePath(string trajectoryName, GameObject robot)
            => $"ros.traj.{trajectoryName}${robot.name}.moveit";
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
            _isValid.Value = _plannedTrajectory.trajectory != null;
            return _isValid.Value;
        }
        public void Move()
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
        private IEnumerator ExecuteTrajectories(MoveToServiceResponse response)
        {
            if (response.trajectory != null)
            {
                foreach (var t in response.trajectory.joint_trajectory.points)
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
                yield return new WaitForSeconds(poseAssignmentWait);
                _runningAction = null;
            }
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
        private MoveToInputMsg GetMoveParameters() => new()
            {
                target_pose = new PoseMsg
                {
                    position = (target - targetOffset - robot.transform.position).To<FLU>(),
                    orientation = targetOrientation.To<FLU>()
                },
                max_velocity = maxVelocity,
                max_acceleration = maxAcceleration,
            };
        private MoveitGroupMsg GetMoveitGroupConfig() => new()
            {
                group_name = groupName,
                end_effector_name = "tool0",
                base_link_name = baseLinkName
            };
    } // class RosMoveitController
} // namespace FSR.DigitalTwin.Client.Features.Robotics.Controller