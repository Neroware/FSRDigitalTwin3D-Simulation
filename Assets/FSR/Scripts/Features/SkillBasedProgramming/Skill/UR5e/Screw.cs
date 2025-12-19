using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.UR5e
{
    public class Screw : OperatorSkillBase
    {
        private readonly Primitive.Delay _delay = new(TimeSpan.FromSeconds(5));
        public override List<IDevicePrimitive> Primitives => new() { _delay };
        public override void Execute(int primitive, object[] input, in SkillResult result)
        {
            _delay.Execute();
        }
        public override async Task ExecuteAsync(int primitive, object[] input, SkillResult result)
        {
            await _delay.ExecuteAsync();
        }
        public override object[] MapPrimitiveInput(int primitive, object[] inputs, object[] inOuts)
        {
            return new object[0];
        }
    }
}