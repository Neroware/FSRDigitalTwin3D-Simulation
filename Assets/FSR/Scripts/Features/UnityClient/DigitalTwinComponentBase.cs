using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.UnityClient {

    public abstract class DigitalTwinComponentBase : MonoBehaviour, IDigitalTwinEntityComponent
    {
        [SerializeField] private string _id = "ex:mycomponent";
        [SerializeField] private DigitalTwinActorBase _actor;
        [SerializeField] private bool _enableOperationModeOverride = false;
        [SerializeField] private DigitalWorkspace.EOperationMode _operationModeOverride = DigitalWorkspace.EOperationMode.Sleep;
        private bool _hasConnection = false;

        public IDigitalTwinEntity DigitalTwinEntity { get => _actor; init => _actor = null; }

        public Uri Id { get => new(_id); init => _id = "ex:mycomponent"; }
        public bool HasConnection => _hasConnection;

        protected virtual bool OnPull() => true;
        protected virtual Task<bool> OnPullAsync() => Task.FromResult(true);
        protected virtual bool OnPush() => true;
        protected virtual Task<bool> OnPushAsync() => Task.FromResult(true);
        protected virtual void OnConnect() { }
        protected virtual void OnDisconnect() { } 

        private DigitalWorkspace.EOperationMode _operationMode =>
            _enableOperationModeOverride ? _operationModeOverride : DigitalWorkspace.Instance.OperationMode;

        private void Start()
        {
            // TODO Adjust!
            bool requestRunning = false;
            Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1.0f))
                .Where(_ => DigitalWorkspace.Instance.Connection.IsConnected.Value)
                .Subscribe(async _ =>
                {
                    if (requestRunning) return;
                    requestRunning = true;
                    switch (_operationMode)
                    {
                        case DigitalWorkspace.EOperationMode.Push: await OnPushAsync(); break;
                        case DigitalWorkspace.EOperationMode.Pull: await OnPullAsync(); break;
                    }
                    requestRunning = false;
                })
                .AddTo(this);
            DigitalWorkspace.Instance.Connection.IsConnected.Where(x => x).Subscribe(_ => OnConnect()).AddTo(this);
            DigitalWorkspace.Instance.Connection.IsConnected.Where(x => !x).Subscribe(_ => OnDisconnect()).AddTo(this);
            OnInitComponent();
        }

        protected virtual void OnInitComponent() { }
    }

}