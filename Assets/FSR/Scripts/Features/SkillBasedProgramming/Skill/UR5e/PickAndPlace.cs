using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.UR5e
{
    public class PickAndPlace : OperatorSkillBase
    {
        [SerializeField] private RosMoveitPickAndPlaceController _controller;

        public override Task<SkillResult> RunAsync(object[] inputs, object[] inOuts)
        {
            return Task.FromResult(new SkillResult() { Succeeded = false });
        }
    }
}