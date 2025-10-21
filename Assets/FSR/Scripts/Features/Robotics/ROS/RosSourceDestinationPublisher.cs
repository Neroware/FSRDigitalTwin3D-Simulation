using System;
using RosMessageTypes.Geometry;
using RosMessageTypes.Ur5eMoveit;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.ROS {

    public class RosSourceDestinationPublisher : RosSourceDestinationPublisherBase
    {
        const int NUM_ROBOT_JOINTS = 6;

        public static readonly string[] LinkNames =
            { "world/base_link/shoulder_link", "/upper_arm_link", "/forearm_link", "/wrist_1_link", "/wrist_2_link", "/wrist_3_link" };

        // Variables required for ROS communication
        [SerializeField] private string _topicName = "/ur5e_joints";
        [SerializeField] private GameObject _ur5e;
        [SerializeField] private GameObject _target;
        [SerializeField] private GameObject _targetPlacement;
        private readonly Quaternion _pickOrientation = Quaternion.Euler(90, 90, 0);

        // Robot Joints
        private UrdfJointRevolute[] _jointArticulationBodies;

        // ROS Connector
        private ROSConnection _rosConn;

        public override string TopicName => _topicName;
        public override GameObject Robot => _ur5e;
        public override GameObject Target => _target;
        public override GameObject TargetPlacement => _targetPlacement;

        private void Start()
        {
            // Get ROS connection static instance
            _rosConn = ROSConnection.GetOrCreateInstance();
            _rosConn.RegisterPublisher<UR5eMoveitJointsMsg>(_topicName);

            _jointArticulationBodies = new UrdfJointRevolute[NUM_ROBOT_JOINTS];

            var linkName = string.Empty;
            for (var i = 0; i < NUM_ROBOT_JOINTS; i++)
            {
                linkName += LinkNames[i];
                _jointArticulationBodies[i] = _ur5e.transform.Find(linkName).GetComponent<UrdfJointRevolute>();
            }
        }

        public override void Publish()
        {
            var sourceDestinationMessage = new UR5eMoveitJointsMsg();

            for (var i = 0; i < NUM_ROBOT_JOINTS; i++)
            {
                sourceDestinationMessage.joints[i] = _jointArticulationBodies[i].GetPosition();
            }

            // Pick Pose
            sourceDestinationMessage.pick_pose = new PoseMsg
            {
                position = _target.transform.position.To<FLU>(),
                orientation = Quaternion.Euler(90, _target.transform.eulerAngles.y, 0).To<FLU>()
            };

            // Place Pose
            sourceDestinationMessage.place_pose = new PoseMsg
            {
                position = _targetPlacement.transform.position.To<FLU>(),
                orientation = _pickOrientation.To<FLU>()
            };

            // Finally send the message to server_endpoint.py running in ROS
            _rosConn.Publish(_topicName, sourceDestinationMessage);
        }
    }

} // END namespace FSR.DigitalTwin.Client.Features.Robotics.ROS