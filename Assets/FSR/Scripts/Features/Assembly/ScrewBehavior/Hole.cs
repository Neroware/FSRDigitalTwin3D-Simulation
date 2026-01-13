using System;
using System.Collections.Generic;
using System.Linq;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.Environment.Interfaces;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using FSR.DigitalTwin.Client.Features.Robotics.Sensor;
using FSR.DigitalTwin.Client.Features.UnityClient;
using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Assembly.ScrewBehavior
{
    public class Hole : MonoBehaviour, IDigitalTwinEntity, ILocation
    {
        [Header("Digital Twin Component")]
        [SerializeField] private string _id;
        [SerializeField] private string _shortId;
        [SerializeField] private List<DigitalTwinComponentBase> _components;
        [Header("Screw Behavior")]
        [SerializeField] private NativeScrewEEController _screwerController;
        [SerializeField] private ColliderSensor _colliderSensor;
        [SerializeField] private Rigidbody _screwPart;

        const string LOCATION_LAYER_NAME = "Location";
        const string DEFAULT_LAYER_NAME = "Default";

        public Uri Id { get => new(_id); init => _id = value.ToString(); }
        public bool HasConnection => false;
        public IEnumerable<IDigitalTwinEntityComponent> Components { get => _components; set => _components = value.Cast<DigitalTwinComponentBase>().ToList(); }
        public Uri LocationId => Id;
        public string LocationName => _shortId;

        public Rigidbody ScrewPart 
        { 
            set 
            {
                if (_screwPart != null) OnScrewPartPick();
                OnScrewPartPlace(value);
            }
            get => _screwPart; 
        }
        public float ScrewPathPercent { set => UpdateScrewPathPercent(value); }

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