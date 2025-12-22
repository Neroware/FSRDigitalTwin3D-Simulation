using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Common.Utils.Semantic;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive
{
    public abstract class GripperBase : IDevicePrimitive
    {
        public string Name { init; get; }
        public Uri Id => UriPrefix.PI + Name;

        public PrimitiveResult Execute(object[] inputs)
            => Execute();
        public async Task<PrimitiveResult> ExecuteAsync(object[] inputs)
            => await ExecuteAsync();
        public abstract PrimitiveResult Execute();
        public abstract Task<PrimitiveResult> ExecuteAsync();
    }
}