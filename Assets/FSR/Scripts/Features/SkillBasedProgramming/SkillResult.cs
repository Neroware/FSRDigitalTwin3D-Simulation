using System;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming
{
    public record SkillResult
    {
        public bool Succeeded { init; get; }
        public bool Failed => !Succeeded;
        public object[] Value { init; get; }
        public TimeSpan TimeExpired { init; get; }
    }
}

