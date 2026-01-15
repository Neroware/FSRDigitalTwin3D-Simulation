using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive
{
    public class Composite : IDevicePrimitive
    {
        public string Name { init; get; }
        public Uri Id => UriPrefix.PI + Name;
        private List<IDevicePrimitive> _primitives;

        public Composite(List<IDevicePrimitive> primitives)
        {
            _primitives = primitives;
        }
        public PrimitiveResult Execute(string task)
        {
            List<PrimitiveResult> tasks = new();
            for(int i = 0; i < _primitives.Count; i++)
            {
                tasks.Add(_primitives[i].Execute(task));
            }
            return new PrimitiveResult()
            {
                Succeeded = tasks.All(x => x.Succeeded),
                Outputs = tasks.Select(x => x.Outputs).ToArray()
            };
        }
        public async Task<PrimitiveResult> ExecuteAsync(string task)
        {
            List<Task<PrimitiveResult>> tasks = new();
            for(int i = 0; i < _primitives.Count; i++)
            {
                tasks.Add(_primitives[i].ExecuteAsync(task));
            }
            var res = await Task.WhenAll(tasks);
            return new PrimitiveResult()
            {
                Succeeded = res.All(x => x.Succeeded),
                Outputs = res.Select(x => x.Outputs).ToArray()
            };
        }
    }
}