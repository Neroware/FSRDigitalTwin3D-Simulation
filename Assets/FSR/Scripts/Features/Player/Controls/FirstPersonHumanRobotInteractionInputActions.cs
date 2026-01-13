using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FSR.DigitalTwin.Client.Features.Player.Controls
{
    public class FirstPersonHumanRobotInteractionInputActions : MonoBehaviour
    {
        private PlayerControls _playerControls;

        private IObservable<Unit> _toggle;
        private IObservable<bool> _grab;
        private IObservable<Vector2> _grabMove;
        public IObservable<Unit> Toggle => _toggle;
        public IObservable<bool> Grab => _grab;
        public IObservable<Vector2> GrabMove => _grabMove;


        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.asset.FindActionMap("FirstPersonHRI").Enable();

            var toggleAction = _playerControls.FindAction("Toggle");
            _toggle =
                Observable.FromEvent<InputAction.CallbackContext>(
                        h => toggleAction.performed += h,
                        h => toggleAction.performed -= h)
                    .AsUnitObservable();
            var grabAction = _playerControls.FindAction("Grab");
            _grab = Observable.EveryUpdate()
                .Select(_ => grabAction.IsPressed())
                .DistinctUntilChanged();
            var _dragAction = _playerControls.FindAction("Drag");
            _grabMove = Observable.EveryUpdate()
                .WithLatestFrom(_grab, (_, isPressed) => isPressed)
                .Where(x => x)
                .Select(_ => _dragAction.ReadValue<Vector2>())
                .Pairwise()
                .Select((tup, _) => tup.Current - tup.Previous)
                .Where(x => x.magnitude > 0);
        }
    }
}
