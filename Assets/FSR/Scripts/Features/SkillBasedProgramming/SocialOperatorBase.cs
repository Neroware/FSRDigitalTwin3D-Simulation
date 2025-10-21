using System;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using FSR.DigitalTwin.Client.Features.UnityClient;
using FSR.DigitalTwin.Client.Features.UnityClient.GRPC;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
using FSR.DigitalTwin.Client.Features.DES;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming
{
    public abstract class SocialOperatorBase : DigitalTwinComponentBase, ISocialOperator
    {
        [SerializeField] private string operatorId = "";
        [SerializeField] private EHRCAgentType agentType = EHRCAgentType.Any;

        public abstract bool IsBusy { get; }
        public abstract string RunningOperation { get; }
        public EHRCAgentType AgentType => agentType;

        protected abstract Task<SkillResult> OnFunction(string function, object[] inputs, object[] inOuts);

        public Uri OperatorId => operatorId.Length == 0 ? Id : new(operatorId);

        protected override void OnConnect()
        {
            DigitalWorkspace.Instance.Operational.ProcessInvoked
                .Where(i => i.OwnerId == Id.ToSafeString())
                .Subscribe(RunFunction).AddTo(this);
        }

        public async Task RunFunctionAsync(ProcessInvocation invocation)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException("Cannot run function because operator is busy!");
            }
            ProcessResult result = new()
            {
                ClientId = GrpcDigitalWorkspaceConnection.UNITY_CLIENT_ID,
                Id = invocation.Id,
                OwnerId = invocation.OwnerId,
                ProcessName = invocation.ProcessName,
                InOuts = new object[0],
                Outputs = new object[] { true },
                TimeStamp = -1
            };
            ProcessExecutionState state = new()
            {
                ClientId = GrpcDigitalWorkspaceConnection.UNITY_CLIENT_ID,
                Id = invocation.Id,
                OwnerId = invocation.OwnerId,
                ProcessName = invocation.ProcessName,
                State = ProcessExecutionState.EState.INITIATED
            };
            var res = await OnFunction(invocation.ProcessName, invocation.Inputs, invocation.InOuts);
            if (res.Failed)
            {
                await DigitalWorkspace.Instance.Operational
                    .SetExecutionProcessStateAsync(state with { State = ProcessExecutionState.EState.FAILED });
                return;
            }
            await DigitalWorkspace.Instance.Operational
                .SetExecutionProcessStateAsync(state with { State = ProcessExecutionState.EState.COMPLETED });
            await DigitalWorkspace.Instance.Operational
                .SetResultAsync(result with
                {
                    InOuts = invocation.InOuts,
                    Outputs = res.Value,
                    TimeStamp = (long) (DateTimeOffset.FromUnixTimeSeconds(
                        invocation.TimeStamp).DateTime + res.TimeExpired).TimeOfDay.TotalSeconds
                });
        }
        public async void RunFunction(ProcessInvocation invocation)
        {
            await RunFunctionAsync(invocation);
        }
        public SkillResult RunFunction(string function, object[] inputs, object[] inOuts)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException("Cannot run function because operator is busy!");
            }
            return OnFunction(function, inputs, inOuts).Result;
        }
        public async Task<SkillResult> RunFunctionAsync(string function, object[] inputs, object[] inOuts)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException("Cannot run function because operator is busy!");
            }
            return await OnFunction(function, inputs, inOuts);
        }
        public HRCProcessResult<HRCFunction> RunFunction(HRCFunction function)
        {
            var res = RunFunction(function.FunctionDescription.FunctionType, function.Inputs, function.InOuts);
            return new HRCProcessResult<HRCFunction>()
            {
                Process = function,
                Succeeded = res.Succeeded,
                TimeStamp = function.Timestamp + res.TimeExpired,
                Outputs = res.Value
            };
        }
        public async Task<HRCProcessResult<HRCFunction>> RunFunctionAsync(HRCFunction function)
        {
            var res = await RunFunctionAsync(function.FunctionDescription.FunctionType, function.Inputs, function.InOuts);
            return new HRCProcessResult<HRCFunction>()
            {
                Process = function,
                Succeeded = res.Succeeded,
                TimeStamp = function.Timestamp + res.TimeExpired,
                Outputs = res.Value
            };
        }

        public abstract bool CanRun(string operation);
        public abstract bool CanRun(Uri operation);
    }

}