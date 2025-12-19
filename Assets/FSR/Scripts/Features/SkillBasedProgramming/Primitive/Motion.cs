using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Common.Utils.Semantic;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive
{
    public abstract class MotionBase : IDevicePrimitive
    {
        public string Name { init; get; }
        public Uri Id => UriPrefix.PI + Name;
        public class MotionResult
        {
            public bool Succeeded { init; get; }
            public double[] Positions { init; get; }
            public static MotionResult Success(double[] positions) => new() { Succeeded = true, Positions = positions };
            public static MotionResult Failure() => new() { Succeeded = false };
        }
        public PrimitiveResult Execute(object[] inputs)
        {
            if (inputs.Length < 0 || inputs[0] is not Vector3 
                || inputs[1] is not Vector3) return PrimitiveResult.Failure("Bad input");
            var result = Execute((Vector3) inputs[0], (Vector3) inputs[1]);
            return new PrimitiveResult { Succeeded = result.Succeeded, Outputs = new object[] { result.Positions }};
        }
        public async Task<PrimitiveResult> ExecuteAsync(object[] inputs)
        {
            if (inputs.Length < 0 || inputs[0] is not Vector3 
                || inputs[1] is not Vector3) return PrimitiveResult.Failure("Bad input");
            
            var result = await ExecuteAsync((Vector3) inputs[0], (Vector3) inputs[1]);
            return new PrimitiveResult { Succeeded = result.Succeeded, Outputs = new object[] { result.Positions }};
        }
        public abstract MotionResult Execute(Vector3 target, Vector3 orientation);
        public abstract Task<MotionResult> ExecuteAsync(Vector3 target, Vector3 orientation);
    }
}