using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive
{
    public abstract class ScrewerToolBase : IDevicePrimitive
    {
        public string Name { init; get; }
        public Uri Id => UriPrefix.PI + Name;
        public double ScrewPathLength { init; get; }
        public double ScrewSpeed { init; get; }
        public abstract PrimitiveResult Execute(string task);
        public abstract Task<PrimitiveResult> ExecuteAsync(string task);
    }
}