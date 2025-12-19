using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.UR5e
{
    public class Screw : OperatorSkillBase
    {
        public override List<IDevicePrimitive> Primitives => throw new System.NotImplementedException();

        public override void Execute(int primitive, object[] input, in SkillResult result)
        {
            Task.Delay(5000).RunSynchronously();
        }
        public override async Task ExecuteAsync(int primitive, object[] input, SkillResult result)
        {
            await Task.Delay(5000);
        }
        public override object[] MapPrimitiveInput(int primitive, object[] inputs, object[] inOuts)
        {
            return new object[0];
        }
    }
}