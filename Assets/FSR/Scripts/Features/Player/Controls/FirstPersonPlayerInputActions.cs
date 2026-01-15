using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FSR.DigitalTwin.Client.Features.Player.Controls
{
    public class FirstPersonPlayerInputActions : MonoBehaviour
    {
        private PlayerControls _playerControls;

        private IObservable<Vector2> _move;
        private IObservable<Vector2> _camera;
        private IObservable<TimeSpan> _jump;
        private IObservable<Unit> _interact;
        private IObservable<Unit> _controlSelect;

        public IObservable<Vector2> Move => _move;
        public IObservable<Vector2> Camera => _camera;
        public IObservable<TimeSpan> Jump => _jump;
        public IObservable<Unit> Interact => _interact;
        public IObservable<Unit> ControlSelect => _controlSelect;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.asset.FindActionMap("FirstPersonPlayer").Enable();
            var moveAction = _playerControls.FindAction("Move");
            var lookAction = _playerControls.FindAction("Look");
            var jumpAction = _playerControls.FindAction("Jump");
            var action0 = _playerControls.FindAction("Action0");
            var controlAction0 = _playerControls.FindAction("Control0");
            
            _move = Observable.EveryUpdate()
                .Select(_ => moveAction.ReadValue<Vector2>())
                .Where(x => x.magnitude > 0);
            
            _camera = Observable.EveryUpdate()
                .Select(_ => lookAction.ReadValue<Vector2>())
                .Where(x => x.magnitude > 0);

            TimeSpan[] jumpDelay = new TimeSpan[]{ TimeSpan.Zero };
            _jump = Observable.EveryUpdate()
                .Select(_ => {
                    jumpDelay[0] = jumpAction.WasPressedThisFrame() ?
                        TimeSpan.Zero : jumpDelay[0] += TimeSpan.FromSeconds(Time.deltaTime);
                    return jumpDelay[0];
                })
                .Where(_ => jumpAction.IsPressed())
                .Share();

            _interact =
                Observable.FromEvent<InputAction.CallbackContext>(
                        h => action0.performed += h,
                        h => action0.performed -= h)
                    .AsUnitObservable();

            _controlSelect =
                Observable.FromEvent<InputAction.CallbackContext>(
                        h => controlAction0.performed += h,
                        h => controlAction0.performed -= h)
                    .AsUnitObservable();
        }
    }
}
