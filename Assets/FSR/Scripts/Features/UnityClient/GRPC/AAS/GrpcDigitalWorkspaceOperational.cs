using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3.Services.Services.SubmodelService;
using FSR.DigitalTwin.Client.Features.UnityClient.GRPC.AAS.Utils;
using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;
using Grpc.Core;
using UniRx;
using Unity.VisualScripting;

namespace FSR.DigitalTwin.Client.Features.UnityClient.GRPC.AAS {

    public class GrpcDigitalWorkspaceOperational : IDigitalWorkspaceOperational {
        
        private readonly Channel _rpcChannel;
        private readonly GrpcAdminShellApiServiceClient _client;
        private static long _counter = 0;
        private static readonly Dictionary<string, string> _handles = new();

        public static string GetHandle(string requestId) => _handles[requestId];

        public IObservable<ProcessInvocation> ProcessInvoked => DigitalWorkspace.Instance.Connection.OnNotify
            .Where(x => x.Type == EClientNotificationType.PROCESS_INVOKED)
            .Select(x => (ProcessInvocation) x);

        public GrpcDigitalWorkspaceOperational(Channel channel) {
            _rpcChannel = channel;
            _client = new(channel);
        }

        public bool RunProcess(string ownerId, string processId, IList<object> input, IList<object> inOut, IList<object> output)
        {
            var inputVars = input.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));
            var inOutVars = inOut.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));

            InvokeOperationSyncRequest request = new() {
                SubmodelId = Base64Converter.ToBase64(ownerId),
                Timestamp = -1,
                RequestId = GrpcDigitalWorkspaceConnection.UNITY_CLIENT_REQUEST_PREFIX + _counter++ + "#" + GrpcDigitalWorkspaceConnection.UNITY_CLIENT_LOCAL_ID
            };

            string[] path = processId.Split('.');
            foreach (string idShort in path) {
                request.Path.Add(new KeyDTO() { Type = KeyTypes.SubmodelElement, Value = idShort });
            }
            request.InputArguments.AddRange(inputVars);
            request.InoutputArguments.AddRange(inOutVars);

            var response = _client.Submodel.InvokeOperationSync(request);
            if (response.StatusCode != (int) HttpStatusCode.OK) {
                return false;
            }
            if (!response.Payload.Success) {
                return false;
            }

            inOut.Clear();
            inOut.AddRange(response.Payload.InoutputArguments.Select(x => x.GetRawValue<object>()));
            output.Clear();
            output.AddRange(response.Payload.OutputArguments.Select(x => x.GetRawValue<object>()));
            return true;
        }

        public async Task<bool> RunProcessAsync(string ownerId, string processId, IList<object> input, IList<object> inOut, IList<object> output)
        {
            var inputVars = input.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));
            var inOutVars = inOut.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));

            InvokeOperationSyncRequest request = new() {
                SubmodelId = Base64Converter.ToBase64(ownerId),
                Timestamp = -1,
                RequestId = GrpcDigitalWorkspaceConnection.UNITY_CLIENT_REQUEST_PREFIX + _counter++ + "#" + GrpcDigitalWorkspaceConnection.UNITY_CLIENT_LOCAL_ID
            };

            string[] path = processId.Split('.');
            foreach (string idShort in path) {
                request.Path.Add(new KeyDTO() { Type = KeyTypes.SubmodelElement, Value = idShort });
            }
            request.InputArguments.AddRange(inputVars);
            request.InoutputArguments.AddRange(inOutVars);

            var response = await _client.Submodel.InvokeOperationSyncAsync(request);
            if (response.StatusCode != (int) HttpStatusCode.OK) {
                return false;
            }
            if (!response.Payload.Success) {
                return false;
            }

            inOut.Clear();
            inOut.AddRange(response.Payload.InoutputArguments.Select(x => x.GetRawValue<object>()));
            output.Clear();
            output.AddRange(response.Payload.OutputArguments.Select(x => x.GetRawValue<object>()));
            return true;
        }

        public long LaunchProcess(string ownerId, string processId, IList<object> input, IList<object> inOut)
        {
            var inputVars = input.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));
            var inOutVars = inOut.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));
            long rid = _counter++;
            string requestId = GrpcDigitalWorkspaceConnection.UNITY_CLIENT_REQUEST_PREFIX + rid + "#" + GrpcDigitalWorkspaceConnection.UNITY_CLIENT_LOCAL_ID;

            InvokeOperationAsyncRequest request = new() {
                SubmodelId = Base64Converter.ToBase64(ownerId),
                Timestamp = -1,
                RequestId = requestId
            };

            string[] path = processId.Split('.');
            foreach (string idShort in path) {
                request.Path.Add(new KeyDTO() { Type = KeyTypes.SubmodelElement, Value = idShort });
            }
            request.InputArguments.AddRange(inputVars);
            request.InoutputArguments.AddRange(inOutVars);

            var response = _client.Submodel.InvokeOperationAsync(request);
            if (response.StatusCode != (int) HttpStatusCode.OK) {
                return -1;
            }
            
            _handles[requestId] = response.Payload;
            return rid;
        }

        public async Task<long> LaunchProcessAsync(string ownerId, string processId, IList<object> input, IList<object> inOut)
        {
            var inputVars = input.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));
            var inOutVars = inOut.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));
            long rid = _counter++;
            string requestId = GrpcDigitalWorkspaceConnection.UNITY_CLIENT_REQUEST_PREFIX + rid + "#" + GrpcDigitalWorkspaceConnection.UNITY_CLIENT_LOCAL_ID;

            InvokeOperationAsyncRequest request = new() {
                SubmodelId = Base64Converter.ToBase64(ownerId),
                Timestamp = -1,
                RequestId = requestId
            };

            string[] path = processId.Split('.');
            foreach (string idShort in path) {
                request.Path.Add(new KeyDTO() { Type = KeyTypes.SubmodelElement, Value = idShort });
            }
            request.InputArguments.AddRange(inputVars);
            request.InoutputArguments.AddRange(inOutVars);

            var response = await _client.Submodel.InvokeOperationAsyncAsync(request);
            if (response.StatusCode != (int) HttpStatusCode.OK) {
                return -1;
            }
            
            _handles[requestId] = response.Payload;
            return rid;
        }

        public bool GetResult(long requestId, IList<object> inOut, IList<object> output)
        {
            string handleId = _handles[GrpcDigitalWorkspaceConnection.UNITY_CLIENT_REQUEST_PREFIX + requestId + "#" + GrpcDigitalWorkspaceConnection.UNITY_CLIENT_LOCAL_ID];
            GetOperationAsyncResultRequest request = new() { HandleId = handleId };

            var response = _client.Submodel.GetOperationAsyncResult(request);
            if (response.StatusCode != (int) HttpStatusCode.OK) {
                return false;
            }
            if (!response.Result.Success) {
                return false;
            }

            inOut.Clear();
            inOut.AddRange(response.Result.InoutputArguments.Select(x => x.GetRawValue<object>()));
            output.Clear();
            output.AddRange(response.Result.OutputArguments.Select(x => x.GetRawValue<object>()));

            return true;
        }

        public async Task<bool> GetResultAsync(long requestId, IList<object> inOut, IList<object> output)
        {
            string handleId = _handles[GrpcDigitalWorkspaceConnection.UNITY_CLIENT_REQUEST_PREFIX + requestId + "#" + GrpcDigitalWorkspaceConnection.UNITY_CLIENT_LOCAL_ID];
            GetOperationAsyncResultRequest request = new() { HandleId = handleId };

            var response = await _client.Submodel.GetOperationAsyncResultAsync(request);
            if (response.StatusCode != (int) HttpStatusCode.OK) {
                return false;
            }
            if (!response.Result.Success) {
                return false;
            }

            inOut.Clear();
            inOut.AddRange(response.Result.InoutputArguments.Select(x => x.GetRawValue<object>()));
            output.Clear();
            output.AddRange(response.Result.OutputArguments.Select(x => x.GetRawValue<object>()));

            return true;
        }

        public bool IsRunning(string ownerId, string processId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRunningAsync(string ownerId, string processId)
        {
            throw new NotImplementedException();
        }

        public bool IsCompleted(string ownerId, string processId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsCompletedAsync(string ownerId, string processId)
        {
            throw new NotImplementedException();
        }

        public bool HasSucceeded(string ownerId, string processId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasSucceededAsync(string ownerId, string processId)
        {
            throw new NotImplementedException();
        }

        public async void SetResult(ProcessResult result)
        {
            await DigitalWorkspace.Instance.Connection.Notify(result);
        }

        public async Task SetResultAsync(ProcessResult result)
        {
            await DigitalWorkspace.Instance.Connection.Notify(result);
        }

        public async void SetExecutionProcessState(ProcessExecutionState executionState)
        {
            await DigitalWorkspace.Instance.Connection.Notify(executionState);
        }

        public async Task SetExecutionProcessStateAsync(ProcessExecutionState executionState)
        {
            await DigitalWorkspace.Instance.Connection.Notify(executionState);
        }
    }

}