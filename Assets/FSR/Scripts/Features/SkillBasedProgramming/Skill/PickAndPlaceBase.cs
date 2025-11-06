using System.Threading.Tasks;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill
{
    public abstract class PickAndPlaceBase : OperatorSkillBase
    {
        public override Task<SkillResult> RunAsync(object[] inputs, object[] inOuts)
        {
            return Task.FromResult(new SkillResult() { Succeeded = false });
        }

        protected abstract Task<bool> RunAsync(Transform pick, Vector3 pickDirection, Transform place, Vector3 placeDirection);
    }
}