using System;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces
{

    public interface ISocialOperator
    {
        bool IsBusy { get; }
        string RunningOperation { get; }
        Uri OperatorId { get; }
        bool CanRun(string operation);
        bool CanRun(Uri operation);
    }

}