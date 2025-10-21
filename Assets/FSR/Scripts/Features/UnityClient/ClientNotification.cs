namespace FSR.DigitalTwin.Client.Features.UnityClient {

    public enum EClientNotificationType {
        EMPTY = 0,
        PROCESS_INVOKED = 1
    }

    public abstract record ClientNotificationBase {
        public abstract EClientNotificationType Type { get; }
    }

    public record ProcessInvocation : ClientNotificationBase {
        public override EClientNotificationType Type => EClientNotificationType.PROCESS_INVOKED;
        public string Id { init; get; }
        public string OwnerId { init; get; }
        public string ProcessName { init; get; }
        public object[] Inputs { init; get; }
        public object[] InOuts { init; get; }
        public long TimeStamp { init; get; }
    }

}