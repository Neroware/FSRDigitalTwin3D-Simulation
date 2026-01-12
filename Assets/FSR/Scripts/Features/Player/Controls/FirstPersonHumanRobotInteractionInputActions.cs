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
        public IObservable<Unit> Toggle => _toggle;

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
        }
    }
}
