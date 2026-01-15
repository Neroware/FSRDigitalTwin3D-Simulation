using System;
using System.Collections.Generic;
using System.Linq;
using FSR.DigitalTwin.Client.Features.Robotics.Sensor;
using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Assembly.ScrewBehavior
{
    public class ScrewPart : WorkPieceBase
    {
        [SerializeField] private ColliderSensor _colliderSensor;
        [SerializeField] private Rigidbody _rigidbody;

        private void Start()
        {
            _colliderSensor.TriggerEntered
                .Where(x => x.TryGetComponent(out Hole _))
                .Select(x => x.GetComponent<Hole>())
                .Subscribe(OnHolePlacement)
                .AddTo(this);
        }
        private void OnHolePlacement(Hole hole)
        {
            hole.ScrewPart = _rigidbody;
        }
    }
}