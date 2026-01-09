using System;
using FSR.DigitalTwin.Client.Features.Robotics.Interfaces;
using FSR.DigitalTwin.Client.Features.UnityClient;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.KinematicRobot {

    public class ScrewerBase : DigitalTwinActorBase, IScrewerTool
    {

        [SerializeField] private float sMax = 1.0f;
        [SerializeField] private float sMin = 0.0f;
        [SerializeField] private ArticulationBody expansion;

        public void PrepareScrew()
        {
            var xDrive = expansion.xDrive;
            xDrive.target = sMax;
            expansion.xDrive = xDrive;
        }
        public void ReleaseScrew()
        {
            var xDrive = expansion.xDrive;
            xDrive.target = sMin;
            expansion.xDrive = xDrive;
        }
        public void SetScrewPercentComplete(float percent)
        {
            var xDrive = expansion.xDrive;
            xDrive.target = Mathf.Lerp(sMin, sMax, Mathf.Clamp(1.0f - percent, 0.0f, 1.0f));
            expansion.xDrive = xDrive;
        }
    }

}