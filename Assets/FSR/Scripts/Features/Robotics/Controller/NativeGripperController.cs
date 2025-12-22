using System;
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

        public override GameObject Robot => _robot;
        public override ReadOnlyReactiveProperty<bool> HasPlanned => _hasPlanned.ToReadOnlyReactiveProperty();
        public override ReadOnlyReactiveProperty<bool> IsValid => _isValid.ToReadOnlyReactiveProperty();
        public override ReadOnlyReactiveProperty<bool> IsInterrupted => _isInterrupted.ToReadOnlyReactiveProperty();
        public override ReadOnlyReactiveProperty<bool> IsRunning => _isRunning.ToReadOnlyReactiveProperty();

        private ReactiveProperty<bool> _hasPlanned = new(false);
        private ReactiveProperty<bool> _isValid = new(false);
        private ReactiveProperty<bool> _isInterrupted = new(false);
        private ReactiveProperty<bool> _isRunning = new(false);

        public IGripperTool Gripper => _gripper;
        public IObservable<IGripperTool> GripperOpened => _gripperSubject
            .Where(x => x == EMode.OPEN).Select(_ => _gripper);
        public IObservable<IGripperTool> GripperClosed => _gripperSubject
            .Where(x => x == EMode.CLOSE).Select(_ => _gripper);

        private Subject<EMode> _gripperSubject = new();

        public void CloseGripper()
        {
            _isValid.Value = true;
            _isRunning.Value = true;
            _isInterrupted.Value = false;
            _gripper.CloseGripper();
            Observable.Timer(TimeSpan.FromSeconds(_gripperDelaySec))
                .First()
                .Subscribe(x => {
                    _gripperSubject.OnNext(EMode.CLOSE);
                    _isRunning.Value = false;
                })
                .AddTo(this);
        }
        public override void ForceInterrupt() { }
        public override bool Interrupt() => true;
        public void OpenGripper()
        {
            _isValid.Value = true;
            _isRunning.Value = true;
            _isInterrupted.Value = false;
            _gripper.OpenGripper();
            Observable.Timer(TimeSpan.FromSeconds(_gripperDelaySec))
                .First()
                .Subscribe(x => {
                    _gripperSubject.OnNext(EMode.OPEN);
                    _isRunning.Value = false;
                })
                .AddTo(this);
        }
        public override void Plan()
        {
            if (_gripper != null)
            {
                _hasPlanned.Value = true;
            }
        }
        public override void RunPlan()
        {
            if (!IsValid.Value) return;
            _isInterrupted.Value = false;
            _isRunning.Value = true;
            if (_mode == EMode.OPEN) OpenGripper();
            else CloseGripper();
            Observable.Timer(TimeSpan.FromSeconds(_gripperDelaySec))
                .First()
                .Subscribe(x => {
                    _gripperSubject.OnNext(_mode);
                    _isRunning.Value = false;
                })
                .AddTo(this);
        }
        public override bool ValidatePlan()
        {
            bool valid = _hasPlanned.Value && _gripper != null;
            _isValid.Value = valid;
            return valid;
        }
        public void SetMode(EMode mode) => _mode = mode;
    }
}