using System;
using System.Linq;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.Environment.Interfaces;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using FSR.DigitalTwin.Client.Features.Robotics.Sensor;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Assembly.ScrewBehavior
{
    public class Hole : WorkPieceBase, ILocation
    {
        [SerializeField] private string _locationName;
        [Header("Screw Behavior")]
        [SerializeField] private NativeScrewEEController _screwerController;
        [SerializeField] private ColliderSensor _colliderSensor;
        [SerializeField] private Rigidbody _screwPart;

        public Uri LocationId => Id;
        public string LocationName => _locationName;

        public Rigidbody ScrewPart 
        { 
            set 
            {
                if (_screwPart != null) 
                    OnScrewPartPick();
                OnScrewPartPlace(value);
                _screwPartSubject.OnNext(value);
            }
            get => _screwPart; 
        }
        public float ScrewPathPercent { set => UpdateScrewPathPercent(value); }

        private readonly Subject<Rigidbody> _screwPartSubject = new();
        public IObservable<Rigidbody> ScrewPartPlaced => _screwPartSubject;


        private void Start()
        {
            if (_screwPart != null) OnScrewPartPlace(_screwPart);
            _screwerController.PercentComplete
                .Where(_ => _screwPart != null && _colliderSensor.SensorData.Value.Colliders.Length > 0)
                .Subscribe(UpdateScrewPathPercent)
                .AddTo(this);
        }
        private void UpdateScrewPathPercent(float value)
        {
            _screwPart.transform.position = transform.position + transform.parent.TransformDirection(
                new Vector3(0.0f, (float)((1.0f - value) * _screwerController.ScrewerPathLength), 0.0f));
        }
        private void OnScrewPartPlace(Rigidbody part)
        {
            _screwPart = part;
            _screwPart.constraints = RigidbodyConstraints.FreezeAll;
            _screwPart.gameObject.layer = LayerMask.NameToLayer(TagNames.LOCATION_LAYER_NAME);
            _screwPart.transform.position = transform.position + transform.parent.TransformDirection(
                new Vector3(0.0f, (float)_screwerController.ScrewerPathLength, 0.0f));
            _screwPart.transform.rotation = transform.rotation;
        }
        private void OnScrewPartPick()
        {
            _screwPart.constraints = RigidbodyConstraints.None;
            _screwPart.gameObject.layer = LayerMask.NameToLayer(TagNames.DEFAULT_LAYER_NAME);
            _screwPart = null;
        }
    }
}