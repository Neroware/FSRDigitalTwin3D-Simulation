using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive
{
    public class IfThen : IDevicePrimitive
    {
        public string Name { init; get; }
        public Uri Id => UriPrefix.PI + Name;

        // Parameter
        public Predicate<string> Predicate { init; get; }
        public IDevicePrimitive Then { init; get; } = Empty.Primitive;
        private IDevicePrimitive Else { init; get; } = Empty.Primitive;

        public IfThen(Predicate<string> predicate)
        {
            Predicate = predicate;
        }
        public PrimitiveResult Execute(string task)
        {
            if (Predicate(task)) return Then.Execute(task);
            return Else.Execute(task);
        }
        public async Task<PrimitiveResult> ExecuteAsync(string task)
        {
            if (Predicate(task)) return await Then.ExecuteAsync(task);
            return await Else.ExecuteAsync(task);
        }
    }
}