using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FSR.DigitalTwin.Client.Features.UnityClient.Interfaces {
    public interface IDigitalWorkspaceOperational {
        IObservable<ProcessInvocation> ProcessInvoked { get; }
        
        bool RunProcess(string ownerId, string processId, IList<object> input, IList<object> inOut, IList<object> output);
        Task<bool> RunProcessAsync(string ownerId, string processId, IList<object> input, IList<object> inOut, IList<object> output);

        long LaunchProcess(string ownerId, string processId, IList<object> input, IList<object> inOut);
        Task<long> LaunchProcessAsync(string ownerId, string processId, IList<object> input, IList<object> inOut);

        bool GetResult(long requestId, IList<object> inOut, IList<object> output);
        Task<bool> GetResultAsync(long requestId, IList<object> inOut, IList<object> output);

        void SetResult(ProcessResult result);
        Task SetResultAsync(ProcessResult result);

        void SetExecutionProcessState(ProcessExecutionState executionState);
        Task SetExecutionProcessStateAsync(ProcessExecutionState executionState);

        bool IsRunning(string ownerId, string processId);
        Task<bool> IsRunningAsync(string ownerId, string processId);

        bool IsCompleted(string ownerId, string processId);
        Task<bool> IsCompletedAsync(string ownerId, string processId);

        public bool HasSucceeded(string ownerId, string processId);
        public Task<bool> HasSucceededAsync(string ownerId, string processId);
    }

}