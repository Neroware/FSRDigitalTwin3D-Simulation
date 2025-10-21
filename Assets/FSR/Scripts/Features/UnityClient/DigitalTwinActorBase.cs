using System;
using System.Collections.Generic;
using System.Linq;
using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.UnityClient {
    public abstract class DigitalTwinActorBase : MonoBehaviour, IDigitalTwinEntity
    {
        [SerializeField] private string _id = "ex:myactor";
        [SerializeField] private List<DigitalTwinComponentBase> _components = new();
        private bool _hasConnection = false;

        public Uri Id { get => new(_id); init => _id = "ex:myactor"; }
        public bool HasConnection => _hasConnection;
        public IEnumerable<IDigitalTwinEntityComponent> Components { 
            get => _components.Cast<IDigitalTwinEntityComponent>(); 
            set => throw new NotImplementedException(); }

        private void Start()
        {
            DigitalWorkspace.Instance?.Connection.IsConnected.Subscribe(OnConnectionChanged).AddTo(this);
            OnInitActor();
        }
        
        protected virtual void OnInitActor() { }

        private async void OnConnectionChanged(bool isConnected) {
            _hasConnection = isConnected && await DigitalWorkspace.Instance.Entities.HasEntityAsync(_id);
        }
    
    }
}

