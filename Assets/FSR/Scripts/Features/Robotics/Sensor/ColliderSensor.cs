using System;
using System.Collections.Generic;
using System.Linq;
using FSR.DigitalTwin.Client.Features.Robotics.Interfaces;
using UniRx;
using UnityEngine;
using static FSR.DigitalTwin.Client.Features.Robotics.Sensor.ColliderSensor;

namespace FSR.DigitalTwin.Client.Features.Robotics.Sensor
{
    public class ColliderSensor : MonoBehaviour, ISensorSource<ColliderSensorData>
    {
        [SerializeField] private string[] _tags = new string[1] { "Untagged" };
        private readonly List<Collider> _colliders = new();
        private readonly ReactiveProperty<ColliderSensorData> _sensorData = new(new()
        {
            Colliders = new Collider[0],
            Trigger = null,
            HasEntered = false
        });
        public ReadOnlyReactiveProperty<ColliderSensorData> SensorData => _sensorData.ToReadOnlyReactiveProperty();
        public IObservable<Collider> TriggerEntered => SensorData
            .Where(x => x.HasEntered && x.Trigger != null && _tags.Contains(x.Trigger.tag))
            .Select(x => x.Trigger);
        public IObservable<Collider> TriggerExited => SensorData
            .Where(x => x.HasExited && x.Trigger != null && _tags.Contains(x.Trigger.tag))
            .Select(x => x.Trigger);

        public record ColliderSensorData
        {
            public Collider[] Colliders { init; get; }
            public Collider Trigger { init; get; }
            public bool HasEntered { init; get; }
            public bool HasExited => !HasEntered;
        }
        public void OnTriggerEnter(Collider other)
        {
            if (!_tags.Contains(other.tag))
                return;
            _colliders.Add(other);
            _sensorData.Value = new()
            {
                Colliders = _colliders.ToArray(),
                Trigger = other,
                HasEntered = true
            };
        }
        public void OnTriggerExit(Collider other)
        {
            if (!_tags.Contains(other.tag))
                return;
            _colliders.Remove(other);
            _sensorData.Value = new()
            {
                Colliders = _colliders.ToArray(),
                Trigger = other,
                HasEntered = false
            };
        }
    }
}