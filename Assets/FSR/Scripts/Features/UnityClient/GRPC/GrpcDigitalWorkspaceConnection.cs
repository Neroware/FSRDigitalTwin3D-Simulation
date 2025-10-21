using System;
using System.Linq;
using System.Threading.Tasks;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3;
using FSR.DigitalTwin.App.GRPC.Services.DigitalTwinClientConnectionService;
using FSR.DigitalTwin.Client.Features.UnityClient.GRPC.AAS;
using FSR.DigitalTwin.Client.Features.UnityClient.GRPC.AAS.Utils;
using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;
using Grpc.Core;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.UnityClient.GRPC {

    public class GrpcDigitalWorkspaceConnection : IDigitalWorkspaceServerConnection
    {
        public ReadOnlyReactiveProperty<bool> IsConnected => _isConnected.ToReadOnlyReactiveProperty();
        public Channel RpcChannel => _rpcChannel ?? throw new RpcException(Status.DefaultCancelled, "No connection established!");
        public IObservable<ClientNotificationBase> OnNotify => _onNotify;

        private Channel _rpcChannel = null;
        private DigitalTwinClientConnectionService.DigitalTwinClientConnectionServiceClient _client = null;
        private AsyncDuplexStreamingCall<ServerNotification, ClientNotification> _notificationStream = null;

        private string _addr;
        private int _port;
        private ReactiveProperty<bool> _isConnected;
        private Subject<ClientNotificationBase> _onNotify;

        public static string UNITY_CLIENT_LOCAL_ID => "FSR.DigitalTwin.Client.Unity::" + DigitalWorkspace.Instance.WorkspaceName;
        public static string UNITY_CLIENT_ID => "https://www.uni-augsburg.de/de/fakultaet/fai/informatik/prof/pi#" + UNITY_CLIENT_LOCAL_ID;
        public static string UNITY_CLIENT_REQUEST_PREFIX => "https://www.uni-augsburg.de/de/fakultaet/fai/informatik/prof/pi?request=";

        public GrpcDigitalWorkspaceConnection(string addr, int port)
        {
            _addr = addr;
            _port = port;
            _isConnected = new(false);
            _onNotify = new();
        }

        public async Task<bool> Connect(string[] connArgs = null)
        {
            _rpcChannel = new Channel(_addr, _port, ChannelCredentials.Insecure);
            _client = new(_rpcChannel);
            // var testMessage = await _client.GetTestMessageAsync(new TestMessage() { Info = "Foo" });
            // Debug.Log(testMessage.Info);

            _notificationStream = _client.Connect();

            ServerNotification connectNotification = new() {
                Type = ServerNotificationType.Connect,
                Connect = new ConnectServerNotification() {
                    ClientId = UNITY_CLIENT_ID
                }
            };
            await _notificationStream.RequestStream.WriteAsync(connectNotification);
            await _notificationStream.ResponseStream.MoveNext();
            var result = _notificationStream.ResponseStream.Current;

            if (result.Connected.Success) {
                _isConnected.Value = true;
                Debug.Log("Connection established!");
            }
            else {
                Debug.LogError("Failed to connect to digital twin workspace!");
            }

            while (await _notificationStream.ResponseStream.MoveNext()) {
                if (_notificationStream.ResponseStream.Current.Type == ClientNotificationType.Aborted)
                    break;
                switch (_notificationStream.ResponseStream.Current.Type) {
                    case ClientNotificationType.InvokeOperation: 
                        OnOperationInvoked(_notificationStream.ResponseStream.Current.InvokeOperation); 
                        break;
                }
                Debug.Log("Client Notification> " + _notificationStream.ResponseStream.Current.Type);
            }
            _isConnected.Value = false;
            _notificationStream = null;
            return true;
        }

        public async Task<bool> Disconnect()
        {
            await _notificationStream.RequestStream.WriteAsync(new ServerNotification() {
                Type = ServerNotificationType.Abort,
                Abort = new AbortServerNotification() { ClientId = UNITY_CLIENT_ID }
            });
            return true;
        }

        public async Task Notify(ServerNotificationBase message) {
            switch(message.Type) {
                case EServerNotificationType.PROCESS_RESULT: await OnOperationResult(message as ProcessResult); break;
                case EServerNotificationType.PROCESS_EXECUTION_STATE: await OnUpdateProcessExecutionState(message as ProcessExecutionState); break;
            }
        }

        public async void Dispose() => await Disconnect();

        private void OnOperationInvoked(InvokeOperationClientNotification notification) {
            _onNotify.OnNext(new ProcessInvocation() {
                Id = notification.RequestId,
                OwnerId = notification.SubmodelId,
                ProcessName = notification.OperationIdShort,
                Inputs = notification.InputVariables.Select(x => x.GetRawValue<object>()).ToArray(),
                InOuts = notification.InoutVariables.Select(x => x.GetRawValue<object>()).ToArray(),
                TimeStamp = notification.Timestamp
            });
        }

        private async Task OnUpdateProcessExecutionState(ProcessExecutionState executionState)
        {
            string handleId = GrpcDigitalWorkspaceOperational.GetHandle(executionState.Id);
            ServerNotification notification = new() {
                Type = ServerNotificationType.OperationState,
                OperationState = new() {
                    HandleId = handleId,
                    ExecutionState = (ExecutionState) executionState.State
                }
            };
            await _notificationStream.RequestStream.WriteAsync(notification);
        }

        private async Task OnOperationResult(ProcessResult processResult) {
            ServerNotification notification = new() { 
                Type = ServerNotificationType.OperationResult, 
                OperationResult = new() { 
                    ClientId = processResult.ClientId, 
                    Result = new() {
                        RequestId = processResult.Id,
                        Success = true,
                        Message = "Marked as finished by digital workspace",
                        ExecutionState = ExecutionState.Completed
                    }
                }
            };
            notification.OperationResult.Result.OutputArguments.AddRange(processResult.Outputs.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x)));
            notification.OperationResult.Result.InoutputArguments.AddRange(processResult.InOuts.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x)));
            await _notificationStream.RequestStream.WriteAsync(notification);
        }
    }

}