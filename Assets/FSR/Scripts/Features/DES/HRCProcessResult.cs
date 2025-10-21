using System;

namespace FSR.DigitalTwin.Client.Features.DES
{
    public record HRCProcessResult
    {
        public HRCProcess Process { init; get; }
        public bool Succeeded { init; get; }
        public bool Failed => !Succeeded;
        public object[] Inputs => Process.Inputs;
        public object[] InOuts => Process.InOuts;
        public object[] Outputs { init; get; }
        public DateTime TimeStamp { init; get; }
    }

    public record HRCProcessResult<T> where T : HRCProcess
    {
        public T Process { init; get; }
        public bool Succeeded { init; get; }
        public bool Failed => !Succeeded;
        public object[] Inputs => Process.Inputs;
        public object[] InOuts => Process.InOuts;
        public object[] Outputs { init; get; }
        public DateTime TimeStamp { init; get; }
        public static implicit operator HRCProcessResult(HRCProcessResult<T> result) => new()
        {
            Process = result.Process,
            Succeeded = result.Succeeded,
            Outputs = result.Outputs,
            TimeStamp = result.TimeStamp
        };
    }
}