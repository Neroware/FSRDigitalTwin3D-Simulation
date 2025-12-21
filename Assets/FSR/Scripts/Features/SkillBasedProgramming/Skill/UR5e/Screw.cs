using System;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.UR5e
{
    public class Screw : OperatorSkillBase
    {
        private readonly Primitive.Delay _delay = new(TimeSpan.FromSeconds(5));
        public override List<IDevicePrimitive> Primitives => new() { _delay };
    }
}