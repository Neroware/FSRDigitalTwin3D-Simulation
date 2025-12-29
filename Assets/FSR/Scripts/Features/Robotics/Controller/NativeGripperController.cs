using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.Robotics.Interfaces;
using FSR.DigitalTwin.Client.Features.Robotics.KinematicRobot;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.Controller
{
    /// <summary>
    /// A native robot controller for very simple gripping end-effectors
    /// </summary>
    public class NativeGripperController : RobotControllerComponent, IGripperController
    {
        [SerializeField] private GameObject _robot;
        [SerializeField] private GripperBase _gripper;
        [SerializeField] private double _gripperDelaySec = 0.5;
        [SerializeField] private EMode _mode;

        public enum EMode
        {
            OPEN, CLOSE
        }
        
        // Controller interface
        public override GameObject Robot => _robot;
        public IGripperTool Gripper => _gripper;

        public override bool Plan()
        {
            return _gripper != null;
        }
        public override Task<bool> PlanAsync()
        {
            return Task.FromResult(Plan());
        }
        public override bool ValidatePlan()
        {
            return _gripper != null;
        }
        public override void RunPlan()
        {
            if (_mode == EMode.OPEN) OpenGripper();
            else CloseGripper();
        }
        public override async Task RunPlanAsync()
        {
            if (_mode == EMode.OPEN) await OpenGripperAsync();
            else await CloseGripperAsync();
        }
        public override bool Interrupt() => true;
        public override void ForceInterrupt() { Interrupt(); }
        public void OpenGripper()
        {
            _gripper.OpenGripper();
            Task.Delay(TimeSpan.FromSeconds(_gripperDelaySec)).RunSynchronously();
        }
        public async Task OpenGripperAsync()
        {
            _gripper.OpenGripper();
            await Task.Delay(TimeSpan.FromSeconds(_gripperDelaySec));
        }
        public void CloseGripper()
        {
            _gripper.CloseGripper();
            Task.Delay(TimeSpan.FromSeconds(_gripperDelaySec)).RunSynchronously();
        }
        public async Task CloseGripperAsync()
        {
            _gripper.CloseGripper();
            await Task.Delay(TimeSpan.FromSeconds(_gripperDelaySec));
        }
        public void SetMode(EMode mode) => _mode = mode;
    }
}