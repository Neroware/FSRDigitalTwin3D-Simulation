using System;
using System.Collections.Generic;
using System.Linq;
using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Assembly
{
    public abstract class WorkPieceBase : MonoBehaviour, IDigitalTwinEntity
    {
        [SerializeField] private string _uri;
        public Uri Id { get => new(_uri); init => throw new NotImplementedException(); }
        public bool HasConnection => false;
        private List<IDigitalTwinEntityComponent> _components = new();
        public IEnumerable<IDigitalTwinEntityComponent> Components { get => _components; set => _components = value.ToList(); }
    }
}