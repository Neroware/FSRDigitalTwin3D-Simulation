using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
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
        [SerializeField] private int jointAssignmentLerpScale = 1;
        [SerializeField] private float jointAssignmentWait = 0.1f;
        [SerializeField] private float poseAssignmentWait = 0.5f;
        [SerializeField] private float maxVelocity = 0.5f;
        [SerializeField] private float maxAcceleration = 0.5f;
        [SerializeField] private string groupName = "ur_manipulator";
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
        [SerializeField] private Vector3 targetOrientation = new(-180, 0, 0);

        // Internal state
        private ArticulationBody[] _jointArticulationBodies;
        private IDisposable _runningAction;
        private MoveToServiceResponse _plannedTrajectory = null;
        private string trajectoryName = "my_trajectory";

        // Parameters
        public string TrajectoryName { set => trajectoryName = value; }
        public Vector3 Target { get => target; set => target = value; }
        public Vector3 TargetOrientation { get => targetOrientation; set => targetOrientation = value; }
        public float MaxVelocity { get => maxVelocity; set => maxVelocity = value; }
        public float MaxAcceleration { get => maxAcceleration; set => maxAcceleration = value; }
        public float JointAssignmentWait { get => jointAssignmentWait; set => jointAssignmentWait = value; }
        public float PoseAssignmentWait { get => poseAssignmentWait; set => poseAssignmentWait = value; }
        public int JointAssignmentLerpScale { get => jointAssignmentLerpScale; set => jointAssignmentLerpScale = value; }

        // Controller interface
        public override GameObject Robot => robot;

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
        public override bool Plan() => PlanAsync().Result;
        public override async Task<bool> PlanAsync()
        {
            MoveToServiceRequest request = new()
            {
                joints_input = GetCurrentJointConfig(),
                group = GetMoveitGroupConfig(),
                pars = GetMoveParameters()
            };
            string filename = GetTrajectoryFilePath(robot, trajectoryName);
            if (!forceMoveItRequest && TrajectoryHelper.IsAvaliable(filename, out MoveToServiceResponse response))
            {
                Debug.Log("response successfully loaded");
                _plannedTrajectory = response;
                return true;
            }
            Debug.Log("Request sent to server");
            _plannedTrajectory = await SendServiceMessageAsync(request);
            TrajectoryHelper.Save(filename, _plannedTrajectory.ToResponseData());
            return true;
        }
        public override bool ValidatePlan()
        {
            return _plannedTrajectory != null && _plannedTrajectory.trajectory != null;
        }
        public override void RunPlan() => RunPlanAsync().RunSynchronously();
        public async override Task RunPlanAsync()
        {
            if (_runningAction != null)
            {
                throw new Exception("should not happen");
            }
            var task = Observable.FromCoroutine(ExecuteTrajectories).ToTask();
            _runningAction = task;
            await task;
            _runningAction = null;
        }
        public override void ForceInterrupt()
        {
            Interrupt();
        }
        public override bool Interrupt()
        {
            _runningAction?.Dispose();
            _runningAction = null;
            return true;
        }
        private Task<MoveToServiceResponse> SendServiceMessageAsync(MoveToServiceRequest request)
        {
            var tcs = new TaskCompletionSource<MoveToServiceResponse>();
            _ros.SendServiceMessage<MoveToServiceResponse>(
                rosServiceName,
                request,
                response =>
                {
                    tcs.SetResult(response);
                }
            );
            return tcs.Task;
        }
        private IEnumerator ExecuteTrajectories()
        {
            MoveToServiceResponse response = _plannedTrajectory;
            if (response.trajectory != null)
            {
                for (int i = 0; i < response.trajectory.joint_trajectory.points.Length - 1; i++)
                {
                    var px0 = response.trajectory.joint_trajectory.points[i].positions;
                    var px1 = response.trajectory.joint_trajectory.points[i + 1].positions;
                    for (int lerpFact = 0; lerpFact < jointAssignmentLerpScale; lerpFact++)
                    {
                        var px = px0
                            .Zip(px1, (x, y) => new Tuple<double, double>(x, y))
                            .Select(p => p.Item1 + ((double) lerpFact / jointAssignmentLerpScale * (p.Item2 - p.Item1)))
                            .ToArray();
                        UpdateJoints(px);
                        yield return new WaitForSeconds(jointAssignmentWait);
                    }
                }
                var end = response.trajectory.joint_trajectory.points.Last().positions;
                UpdateJoints(end);
                yield return new WaitForSeconds(poseAssignmentWait);
            }
        }

        private void UpdateJoints(double[] jointPositions)
        {
            var result = jointPositions.Select(r => (float)r * Mathf.Rad2Deg).ToArray();
            for (var joint = 0; joint < _jointArticulationBodies.Length; joint++)
            {
                var joint1XDrive = _jointArticulationBodies[joint].xDrive;
                joint1XDrive.target = result[joint];
                _jointArticulationBodies[joint].xDrive = joint1XDrive;
            }
        }

        private static string GetTrajectoryFilePath(GameObject robot, string trajectoryName)
            => $"ros2.{robot.name}.{trajectoryName}.moveit";

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
                    orientation = Quaternion.Euler(targetOrientation).To<FLU>()
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