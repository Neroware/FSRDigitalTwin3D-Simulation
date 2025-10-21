// =================================================================================================================================== //
// Modified version of https://github.com/louis1218/digitaltwins-Unity/blob/main/Assets/ZeroMQ/Controller/handwave/MotionPlanning.cs
// by louis1218
//
// For more information visit https://github.com/louis1218/digitaltwins-Unity
// =================================================================================================================================== //

using System;
using UnityEngine;

namespace Unity.Robotics.UrdfImporter.Control
{
    public enum RotationDirection { None = 0, Positive = 1, Negative = -1 };
    public enum ControlType { PositionControl };
    public enum ShoulderChainSide { Right = 0, Left = 1 };

    public class PepperUrdfController : UrdfController
    {
        private ArticulationBody[] LShoulderChain;
        private ArticulationBody[] RShoulderChain;

        [InspectorReadOnly(hideInEditMode: true)] public string selectedJoint;
        [HideInInspector] [SerializeField] private ControlType control = ControlType.PositionControl;
        public float stiffness;
        public float damping;
        public float forceLimit;
        public float speed; // Units: degree/s
        public float torque; // Units: Nm or N
        public float acceleration;// Units: m/s^2 / degree/s^2
        public int defDyanmicVal = 10;

        // Right Arm
        [SerializeField] private GameObject Rshoulder; 
        [SerializeField] private GameObject RBicep;
        [SerializeField] private GameObject RForearm;
        [SerializeField] private GameObject Rwrist;

        private ArticulationBody rshoulder_ab;
        private ArticulationBody rforearm_ab;
        private ArticulationBody rwrist_ab;
        private ArticulationBody rbicep_ab;

        // Left Arm
        [SerializeField] private GameObject Lshoulder;
        [SerializeField] private GameObject LBicep;
        [SerializeField] private GameObject LForearm;
        [SerializeField] private GameObject Lwrist;

        private ArticulationBody lshoulder_ab;
        private ArticulationBody lforearm_ab;
        private ArticulationBody lwrist_ab;
        private ArticulationBody lbicep_ab;
        
        // Base joint indices
        private int shoulderIndex;
        private int bicepIndex;
        private int forearmIndex;
        private int wristIndex;

        // Right Fingers
        [SerializeField] private GameObject RThumb1; 
        [SerializeField] private GameObject RFinger11;
        [SerializeField] private GameObject RFinger21;
        [SerializeField] private GameObject RFinger31;
        [SerializeField] private GameObject RFinger41;

        private ArticulationBody rthumb1_ab;
        private ArticulationBody rfinger11_ab;
        private ArticulationBody rfinger21_ab;
        private ArticulationBody rfinger31_ab;
        private ArticulationBody rfinger41_ab;

        // Left Fingers
        [SerializeField] private GameObject LThumb1; 
        [SerializeField] private GameObject LFinger11;
        [SerializeField] private GameObject LFinger21;
        [SerializeField] private GameObject LFinger31;
        [SerializeField] private GameObject LFinger41;

        private ArticulationBody lthumb1_ab;
        private ArticulationBody lfinger11_ab;
        private ArticulationBody lfinger21_ab;
        private ArticulationBody lfinger31_ab;
        private ArticulationBody lfinger41_ab;

        // Finger indices
        private int thumb1Index;
        private int finger11Index;
        private int finger21Index;
        private int finger31Index;
        private int finger41Index;

        [SerializeField] private string moveAction = "";

        public override float Stiffness { get => stiffness; set => stiffness = value; }
        public override float Damping { get => damping; set => damping = value; }
        public override float ForceLimit { get => forceLimit; set => forceLimit = value; }
        public override float Speed { get => speed; set => speed = value; }
        public override float Torque { get => torque; set => torque = value; }
        public override float Acceleration { get => acceleration; set => acceleration = value; }
        public override int DefDyanmicVal { get => defDyanmicVal; set => defDyanmicVal = value; }

        private void InitialiseJointPhysics(ArticulationBody[] bodyChain){
            foreach (ArticulationBody joint in bodyChain)
            {
                joint.gameObject.AddComponent<JointControl>();
                joint.jointFriction = defDyanmicVal;
                joint.angularDamping = defDyanmicVal;
                ArticulationDrive currentDrive = joint.xDrive;
                currentDrive.forceLimit = forceLimit;
                joint.xDrive = currentDrive;
            }            
        }

        private void Start()
        {
            rshoulder_ab = Rshoulder.GetComponent<ArticulationBody>();
            rforearm_ab = RForearm.GetComponent<ArticulationBody>();
            rwrist_ab = Rwrist.GetComponent<ArticulationBody>();
            rbicep_ab = RBicep.GetComponent<ArticulationBody>();
            rthumb1_ab = RThumb1.GetComponent<ArticulationBody>();
            rfinger11_ab = RFinger11.GetComponent<ArticulationBody>();
            rfinger21_ab = RFinger21.GetComponent<ArticulationBody>();
            rfinger31_ab = RFinger31.GetComponent<ArticulationBody>();
            rfinger41_ab = RFinger41.GetComponent<ArticulationBody>();

            lshoulder_ab = Lshoulder.GetComponent<ArticulationBody>();
            lforearm_ab = LForearm.GetComponent<ArticulationBody>();
            lwrist_ab = Lwrist.GetComponent<ArticulationBody>();
            lbicep_ab = LBicep.GetComponent<ArticulationBody>();
            lthumb1_ab = LThumb1.GetComponent<ArticulationBody>();
            lfinger11_ab = LFinger11.GetComponent<ArticulationBody>();
            lfinger21_ab = LFinger21.GetComponent<ArticulationBody>();
            lfinger31_ab = LFinger31.GetComponent<ArticulationBody>();
            lfinger41_ab = LFinger41.GetComponent<ArticulationBody>();

            shoulderIndex = 0;
            bicepIndex = 1;
            forearmIndex = 3;
            wristIndex = 4;
            thumb1Index = 5;
            finger11Index = 7;
            finger21Index = 10;
            finger31Index = 13;
            finger41Index = 16;

            this.gameObject.AddComponent<FKRobot>();
            RShoulderChain = rshoulder_ab.GetComponentsInChildren<ArticulationBody>();
            LShoulderChain = lshoulder_ab.GetComponentsInChildren<ArticulationBody>();

            InitialiseJointPhysics(RShoulderChain);
            InitialiseJointPhysics(LShoulderChain);
        }

        private void Update()
        {
            switch (moveAction) {
                case "origin": {
                    ShoulderReturnToOrigin(shoulderIndex, ShoulderChainSide.Right);
                    ShoulderReturnToOrigin(shoulderIndex, ShoulderChainSide.Left);
                    BicepReturnToOrigin(bicepIndex, ShoulderChainSide.Right);
                    BicepReturnToOrigin(bicepIndex, ShoulderChainSide.Left);
                    WristReturnToOrigin(wristIndex, ShoulderChainSide.Right);
                    WristReturnToOrigin(wristIndex, ShoulderChainSide.Left);
                    ForeArmReturnToOrigin(forearmIndex, ShoulderChainSide.Right);
                    ForeArmReturnToOrigin(forearmIndex, ShoulderChainSide.Left);
                } break;
                default: {
                    BlockJoints();
                } break;
            }
        }

        private void BlockJoints() {
            foreach (int index in new int[]{wristIndex, bicepIndex, forearmIndex, shoulderIndex}) {
                RShoulderChain[index].GetComponent<JointControl>().direction = RotationDirection.None;
                LShoulderChain[index].GetComponent<JointControl>().direction = RotationDirection.None;
            }
        }

        private void WristReturnToOrigin(int jointIndex, ShoulderChainSide side){
            JointControl current_wrist = (side == ShoulderChainSide.Right ? RShoulderChain : LShoulderChain)[jointIndex]
                .GetComponent<JointControl>();
            ArticulationDrive wristDrive = (side == ShoulderChainSide.Right ? rwrist_ab : lwrist_ab).xDrive;
            var current_target  = wristDrive.target;
            if (current_target > 0) {
                current_wrist.direction = side == ShoulderChainSide.Right ? RotationDirection.Negative : RotationDirection.Positive;
            }
            if (current_target < 0){
                current_wrist.direction = side == ShoulderChainSide.Right ? RotationDirection.Positive : RotationDirection.Negative;
            }
            if (current_target < 0 || current_target == 0) {
                current_wrist.direction = RotationDirection.None;
            }  
        }

        private void BicepReturnToOrigin(int jointIndex, ShoulderChainSide side){
            JointControl current_bicep = (side == ShoulderChainSide.Right ? RShoulderChain : LShoulderChain)[jointIndex]
                .GetComponent<JointControl>();
            ArticulationDrive bicepDrive = (side == ShoulderChainSide.Right ? rbicep_ab : lbicep_ab).xDrive;
            var current_target  = bicepDrive.target;
            if (current_target < 0){
                current_bicep.direction = RotationDirection.Positive;
            }
            else if (current_target > 0){
                current_bicep.direction = RotationDirection.Negative;
            }
            else if (current_target == 0){
                current_bicep.direction = RotationDirection.None;
            }       
        }

        private void ForeArmReturnToOrigin(int jointIndex, ShoulderChainSide side){
            JointControl current_hw = (side == ShoulderChainSide.Right ? RShoulderChain : LShoulderChain)[jointIndex]
                .GetComponent<JointControl>();
            ArticulationDrive forearmDrive = (side == ShoulderChainSide.Right ? rforearm_ab : lforearm_ab).xDrive;
            var current_target  = forearmDrive.target;
            if (current_target > 0){
                current_hw.direction = RotationDirection.Negative;
            }
            if (current_target < 0){
                current_hw.direction = RotationDirection.Positive;
            }
            if (current_target == 0){
                current_hw.direction = RotationDirection.None;
            }
        }

        private void ShoulderReturnToOrigin(int jointIndex, ShoulderChainSide side){
            JointControl current_hw = (side == ShoulderChainSide.Right ? RShoulderChain : LShoulderChain)[jointIndex]
                .GetComponent<JointControl>();
            ArticulationDrive shoulderDrive = (side == ShoulderChainSide.Right ? rshoulder_ab : lshoulder_ab).xDrive;
            var current_target  = shoulderDrive.target;
            if (current_target < 0){
                current_hw.direction = RotationDirection.Positive;
            }
            else if (current_target > 0){
                current_hw.direction = RotationDirection.Negative;
            }
            else if (current_target == 0){
                current_hw.direction = RotationDirection.None;
            }
        }

        public override void UpdateControlType(JointControl joint)
        {
            joint.controltype = control;
            if (control == ControlType.PositionControl)
            {
                ArticulationDrive drive = joint.joint.xDrive;
                drive.stiffness = stiffness;
                drive.damping = damping;
                joint.joint.xDrive = drive;
            }
        }

        void DestroyScriptInstance()
        {
            // Removes this script instance from the game object
            Destroy(this);
            print("MotionPlan Done");
        }
    }
}