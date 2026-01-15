using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive
{
    public abstract class MotionBase : IDevicePrimitive
    {
        public string Name { init; get; }
        public Uri Id => UriPrefix.PI + Name;

        // Parameters
        public Vector3 Target { init; get; }
        public Vector3 Orientation { init; get; }

        public abstract PrimitiveResult Execute(string task);
        public abstract Task<PrimitiveResult> ExecuteAsync(string task);
    }
}