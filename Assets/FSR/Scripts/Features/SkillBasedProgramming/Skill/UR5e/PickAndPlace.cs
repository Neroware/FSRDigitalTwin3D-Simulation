using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.UR5e
{
    public class PickAndPlace : OperatorSkillBase
    {
        [SerializeField] private RosMoveitPickAndPlaceController _controller;

        public override List<IDevicePrimitive> Primitives => throw new System.NotImplementedException();

        public override void Execute(int primitive, object[] input, in SkillResult result)
        {
            throw new System.NotImplementedException();
        }
        public override Task ExecuteAsync(int primitive, object[] input, SkillResult result)
        {
            throw new System.NotImplementedException();
        }
        public override object[] MapPrimitiveInput(int primitive, object[] inputs, object[] inOuts)
        {
            throw new System.NotImplementedException();
        }

        // public override Task<SkillResult> RunAsync(object[] inputs, object[] inOuts)
        // {
        //     return Task.FromResult(new SkillResult() { Succeeded = false });
        // }
    }
}