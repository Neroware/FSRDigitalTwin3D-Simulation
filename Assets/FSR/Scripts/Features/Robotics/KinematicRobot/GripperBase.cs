using FSR.DigitalTwin.Client.Features.Robotics.Interfaces;
using FSR.DigitalTwin.Client.Features.UnityClient;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.KinematicRobot {

    public class GripperBase : DigitalTwinActorBase, IGripperTool
    {

        [SerializeField] private float sMax = 1.0f;
        [SerializeField] private float sMin = 0.0f;

        [SerializeField] private ArticulationBody fingerL;
        [SerializeField] private ArticulationBody fingerR;

        public bool Opened => fingerL.xDrive.target != -sMin || fingerR.xDrive.target != sMin;
        public void CloseGripper()
        {
            if (!fingerL || !fingerR) {
                Debug.LogWarning("No articulation bodies set for gripper functionality!");
                return;
            }

            var xDriveL = fingerL.xDrive;
            var xDriveR = fingerR.xDrive;

            xDriveL.target = -sMin;
            xDriveR.target = sMin;

            fingerL.xDrive = xDriveL;
            fingerR.xDrive = xDriveR;
        }

        public void OpenGripper()
        {
            if (!fingerL || !fingerR) {
                Debug.LogWarning("No articulation bodies set for gripper functionality!");
                return;
            }

            var xDriveL = fingerL.xDrive;
            var xDriveR = fingerR.xDrive;

            xDriveL.target = -sMax;
            xDriveR.target = sMax;

            fingerL.xDrive = xDriveL;
            fingerR.xDrive = xDriveR;
        }
    }

}