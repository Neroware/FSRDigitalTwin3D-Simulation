using System.Threading.Tasks;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.UR5e
{
    public class Screw : OperatorSkillBase
    {
        public override async Task<SkillResult> RunAsync(object[] inputs, object[] inOuts)
        {
            await Task.Delay(5000);
            return new SkillResult() { Succeeded = false };
        }
    }
}