using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive
{
    public class Delay : IDevicePrimitive
    {
        public string Name { init; get; }
        public Uri Id => UriPrefix.PI + Name;

        // Parameters
        public TimeSpan Time { init; get; } = TimeSpan.Zero;

        public PrimitiveResult Execute(string _task)
        {
            Task.Delay(Time).RunSynchronously();
            return new PrimitiveResult { Succeeded = true, Outputs = new object[0]};
        }
        public async Task<PrimitiveResult> ExecuteAsync(string _task)
        {
            await Task.Delay(Time);
            return new PrimitiveResult { Succeeded = true, Outputs = new object[0]};
        }
    }
}