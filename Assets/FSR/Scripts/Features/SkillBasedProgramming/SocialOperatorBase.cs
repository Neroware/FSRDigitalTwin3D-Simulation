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

        protected abstract Task<SkillResult> OnRun(string operation, object[] inputs, object[] inOuts);

        public Uri OperatorId => operatorId.Length == 0 ? Id : new(operatorId);

        protected override void OnConnect()
        {
            DigitalWorkspace.Instance.Operational.ProcessInvoked
                .Where(i => i.OwnerId == Id.ToSafeString())
                .Subscribe(RunOperation).AddTo(this);
        }

        public async Task RunOperationAsync(ProcessInvocation invocation)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException("Cannot run operation because operator is busy!");
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
            var res = await OnRun(invocation.ProcessName, invocation.Inputs, invocation.InOuts);
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
                    Outputs = res.Outputs,
                    TimeStamp = (long) (DateTimeOffset.FromUnixTimeSeconds(
                        invocation.TimeStamp).DateTime + res.TimeExpired).TimeOfDay.TotalSeconds
                });
        }
        public async void RunOperation(ProcessInvocation invocation)
        {
            await RunOperationAsync(invocation);
        }
        public SkillResult RunOperation(string operation, object[] inputs, object[] inOuts)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException("Cannot run operation because operator is busy!");
            }
            return OnRun(operation, inputs, inOuts).Result;
        }
        public async Task<SkillResult> RunOperationAsync(string operation, object[] inputs, object[] inOuts)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException("Cannot run operation because operator is busy!");
            }
            return await OnRun(operation, inputs, inOuts);
        }
        public HRCProcessResult<HRCFunction> RunOperation(string operation, HRCFunction function)
        {
            var res = RunOperation(operation, function.Inputs, function.InOuts);
            return new HRCProcessResult<HRCFunction>()
            {
                Process = function,
                Succeeded = res.Succeeded,
                TimeStamp = function.Timestamp + res.TimeExpired,
                Outputs = res.Outputs
            };
        }
        public async Task<HRCProcessResult<HRCFunction>> RunOperationAsync(string operation, HRCFunction function)
        {
            var res = await RunOperationAsync(operation, function.Inputs, function.InOuts);
            return new HRCProcessResult<HRCFunction>()
            {
                Process = function,
                Succeeded = res.Succeeded,
                TimeStamp = function.Timestamp + res.TimeExpired,
                Outputs = res.Outputs
            };
        }

        public abstract bool CanRun(string operation);
        public abstract bool CanRun(Uri operation);
    }

}