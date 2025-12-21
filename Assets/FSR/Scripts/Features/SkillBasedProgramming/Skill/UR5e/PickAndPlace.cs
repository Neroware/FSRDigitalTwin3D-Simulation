using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.UR5e
{
    public class PickAndPlace : PickAndPlaceBase
    {
        [SerializeField] private RosMoveitPickAndPlaceController _controller;
        public override List<IDevicePrimitive> Primitives => throw new System.NotImplementedException();
        public override SkillResult Run(Vector3 pickPosition, Vector3 pickOrientation, Vector3 placePosition, Vector3 placeOrientation)
        {
            throw new System.NotImplementedException();
        }
        public override Task<SkillResult> RunAsync(Vector3 pickPosition, Vector3 pickOrientation, Vector3 placePosition, Vector3 placeOrientation)
        {
            throw new System.NotImplementedException();
        }
    }
}