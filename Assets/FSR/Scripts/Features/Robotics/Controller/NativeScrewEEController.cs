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
    public class NativeScrewEEController : RobotControllerComponent, IScrewerController
    {
        [SerializeField] private GameObject _robot;
        [SerializeField] private ScrewerBase _screwerTool;
        [SerializeField] private double _screwerSpeed = 1.0;
        [SerializeField] private double _screwerPathLength = 1.0;
        [SerializeField] private EMode _mode;

        public enum EMode
        {
            IN, OUT
        }
        
        // Controller interface
        public override GameObject Robot => _robot;
        public IScrewerTool ScrewerTool => _screwerTool;

        public override bool Plan()
        {
            return _screwerTool != null;
        }
        public override Task<bool> PlanAsync()
        {
            return Task.FromResult(Plan());
        }
        public override bool ValidatePlan() 
            => _screwerTool != null && _screwerPathLength > 0.0 && _screwerSpeed > 0.0;
        public override void RunPlan()
        {
            if (_mode == EMode.OUT) ScrewOut();
            else ScrewIn();
        }
        public override async Task RunPlanAsync()
        {
            if (_mode == EMode.OUT) await ScrewOutAsync();
            else await ScrewInAsync();
        }
        public override bool Interrupt() => true;
        public override void ForceInterrupt() { Interrupt(); }
        public void SetMode(EMode mode) => _mode = mode;

        public void PrepareScrew()
        {
            throw new NotImplementedException();
        }
        public Task PrepareScrewAsync()
        {
            throw new NotImplementedException();
        }
        public void ReleaseScrew()
        {
            throw new NotImplementedException();
        }
        public Task ReleaseScrewAsync()
        {
            throw new NotImplementedException();
        }
        public void ScrewIn()
        {
            throw new NotImplementedException();
        }
        public Task ScrewInAsync()
        {
            throw new NotImplementedException();
        }
        public void ScrewOut()
        {
            throw new NotImplementedException();
        }
        public Task ScrewOutAsync()
        {
            throw new NotImplementedException();
        }
        public void StopScrewer()
        {
            throw new NotImplementedException();
        }
    }
}