using System;
using System.Collections.Generic;
using System.Linq;
using FSR.DigitalTwin.Client.Features.Robotics.Sensor;
using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;
using UniRx;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace FSR.DigitalTwin.Client.Features.Assembly.ScrewBehavior
{
    public class VRScrewPart : WorkPieceBase
    {
        [SerializeField] private ColliderSensor _colliderSensor;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;
        [SerializeField] private XRGrabInteractable _xrGrabInteractable;

        private void Start()
        {
            _colliderSensor.TriggerEntered
                .Where(x => x.TryGetComponent(out Hole _))
                .Select(x => x.GetComponent<Hole>())
                .Subscribe(OnHolePlacement)
                .AddTo(this);

            Observable.EveryUpdate()
                .Select(_ => _xrGrabInteractable.isSelected)
                .DistinctUntilChanged()
                .Subscribe(x => _collider.enabled = !x)
                .AddTo(this);
        }
        private void OnHolePlacement(Hole hole)
        {
            hole.ScrewPart = _rigidbody;
        }
    }
}