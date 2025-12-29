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
        private readonly TimeSpan _delay;
        public Delay(TimeSpan delay)
        {
            _delay = delay;
        }
        public PrimitiveResult Execute(string task, object[] inputs)
        {
            Task.Delay(_delay).RunSynchronously();
            return new PrimitiveResult { Succeeded = true, Outputs = new object[0]};
        }
        public async Task<PrimitiveResult> ExecuteAsync(string task, object[] inputs)
        {
            await Task.Delay(_delay);
            return new PrimitiveResult { Succeeded = true, Outputs = new object[0]};
        }
        public PrimitiveResult Execute()
        {
            Task.Delay(_delay).RunSynchronously();
            return new PrimitiveResult { Succeeded = true, Outputs = new object[0]};
        }
        public async Task<PrimitiveResult> ExecuteAsync()
        {
            await Task.Delay(_delay);
            return new PrimitiveResult { Succeeded = true, Outputs = new object[0]};
        }
    }
}