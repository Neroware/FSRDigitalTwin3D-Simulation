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
        [SerializeField] private int _screwerPrepareMilliseconds = 500;
        [SerializeField] private EMode _mode;

        private ReactiveProperty<float> _percentComplete = new(0.0f);
        public ReadOnlyReactiveProperty<float> PercentComplete => _percentComplete.ToReadOnlyReactiveProperty();

        private bool isRunning = false;

        public enum EMode
        {
            IN, OUT
        }
        
        // Controller interface
        public override GameObject Robot => _robot;
        public IScrewerTool ScrewerTool => _screwerTool;
        
        // Parameters
        public double ScrewerSpeed { get => _screwerSpeed; set => _screwerSpeed = value; }
        public double ScrewerPathLength { get => _screwerPathLength; set => _screwerPathLength = value; }
        public int ScrewerPrepareMilliseconds { get => _screwerPrepareMilliseconds; set => _screwerPrepareMilliseconds = value; }

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
        public override bool Interrupt() 
        { 
            StopScrewer(); 
            return true;
        } 
        public override void ForceInterrupt() => Interrupt();
        public void SetMode(EMode mode) => _mode = mode;

        public void PrepareScrew() => _screwerTool.PrepareScrew();
        public async Task PrepareScrewAsync()
        {
            _screwerTool.PrepareScrew();
            await Task.Delay(_screwerPrepareMilliseconds);
        }
        public void ReleaseScrew() => _screwerTool.ReleaseScrew();
        public async Task ReleaseScrewAsync()
        {
            _screwerTool.ReleaseScrew();
            await Task.Delay(_screwerPrepareMilliseconds);
        }
        public void ScrewIn()
        {
            if (isRunning)
                throw new OperationCanceledException("Action already performed by controller.");
            isRunning = true;
            _percentComplete.Value = 0.0f;
            Observable.EveryUpdate()
                .TakeWhile(_ => isRunning)
                .Subscribe(_ => {
                    _screwerTool.SetScrewPercentComplete(_percentComplete.Value);
                    _percentComplete.Value += (float)(_screwerSpeed * Time.deltaTime / _screwerPathLength);
                    isRunning = _percentComplete.Value < 1.0f;
                })
                .AddTo(this);
        }
        public async Task ScrewInAsync()
        {
            if (isRunning)
                throw new OperationCanceledException("Action already performed by controller.");
            isRunning = true;
            _percentComplete.Value = 0.0f;
            var obs = Observable.EveryUpdate().TakeWhile(_ => isRunning);
            obs.Subscribe(_ => {
                    _screwerTool.SetScrewPercentComplete(_percentComplete.Value);
                    _percentComplete.Value += (float)(_screwerSpeed * Time.deltaTime / _screwerPathLength);
                    isRunning = _percentComplete.Value < 1.0f;
                })
                .AddTo(this);
            await obs.ToTask();
        }
        public void ScrewOut()
        {
            if (isRunning)
                throw new OperationCanceledException("Action already performed by controller.");
            isRunning = true;
            _percentComplete.Value = 1.0f;
            Observable.EveryUpdate()
                .TakeWhile(_ => isRunning)
                .Subscribe(_ => {
                    _screwerTool.SetScrewPercentComplete(_percentComplete.Value);
                    _percentComplete.Value -= (float)(_screwerSpeed * Time.deltaTime / _screwerPathLength);
                    isRunning = _percentComplete.Value > 0.0f;
                })
                .AddTo(this);
        }
        public async Task ScrewOutAsync()
        {
            if (isRunning)
                throw new OperationCanceledException("Action already performed by controller.");
            isRunning = true;
            _percentComplete.Value = 1.0f;
            var obs = Observable.EveryUpdate().TakeWhile(_ => isRunning);
            obs.Subscribe(_ => {
                    _screwerTool.SetScrewPercentComplete(_percentComplete.Value);
                    _percentComplete.Value -= (float)(_screwerSpeed * Time.deltaTime / _screwerPathLength);
                    isRunning = _percentComplete.Value > 0.0f;
                })
                .AddTo(this);
            await obs.ToTask();
        }
        public void StopScrewer()
        {
            isRunning = false;
        }
    }
}