using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive
{
    public abstract class GripperBase : IDevicePrimitive
    {
        public string Name { init; get; }
        public Uri Id => UriPrefix.PI + Name;

        public PrimitiveResult Execute(string task, object[] inputs)
            => Execute(task);
        public async Task<PrimitiveResult> ExecuteAsync(string task, object[] inputs)
            => await ExecuteAsync(task);
        public abstract PrimitiveResult Execute(string task);
        public abstract Task<PrimitiveResult> ExecuteAsync(string task);
    }
}