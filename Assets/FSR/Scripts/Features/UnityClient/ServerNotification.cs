namespace FSR.DigitalTwin.Client.Features.UnityClient {

    public enum EServerNotificationType {
        EMPTY = 0,
        PROCESS_RESULT = 1,
        PROCESS_EXECUTION_STATE = 2
    }

    public abstract record ServerNotificationBase {
        public abstract EServerNotificationType Type { get; }
        public string ClientId { get; init; }
    }

    public record ProcessResult : ServerNotificationBase
    {
        public override EServerNotificationType Type => EServerNotificationType.PROCESS_RESULT;

        public string Id { init; get; }
        public string OwnerId { init; get; }
        public string ProcessName { init; get; }
        public object[] InOuts { init; get; }
        public object[] Outputs { init; get; }
        public long TimeStamp { init; get; }

    }

    public record ProcessExecutionState : ServerNotificationBase
    {
        public enum EState {
            INITIATED = 0, RUNNING = 1, COMPLETED = 2, CANCELED = 3, FAILED = 4, TIMEOUT = 5
        }

        public override EServerNotificationType Type => EServerNotificationType.PROCESS_EXECUTION_STATE;

        public string Id { init; get; }
        public string OwnerId { init; get; }
        public string ProcessName { init; get; }
        public EState State { init; get; }
    }

}