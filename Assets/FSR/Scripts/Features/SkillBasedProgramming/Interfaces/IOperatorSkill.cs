using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces
{
    public interface IOperatorSkill
    {
        string[] ShortIds { get; }
        Uri Id { get; }
        SkillResult Run(string task, object[] inputs, object[] inOuts);
        Task<SkillResult> RunAsync(string task, object[] inputs, object[] inOuts);
        List<IDevicePrimitive> Primitives { get; }
    }
}

