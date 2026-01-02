using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive
{
    public class Empty : IDevicePrimitive
    {
        public string Name { init; get; }
        public Uri Id => UriPrefix.PI + Name;
        public static Empty Primitive => new() { Name = "pi:empty"};
        public PrimitiveResult Execute(string _task)
        {
            return new PrimitiveResult { Succeeded = true, Outputs = new object[0]};
        }
        public async Task<PrimitiveResult> ExecuteAsync(string _task)
        {
            return new PrimitiveResult { Succeeded = true, Outputs = new object[0]};
        }
    }
}