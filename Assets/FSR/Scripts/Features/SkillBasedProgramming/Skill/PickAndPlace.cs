using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill
{
    public abstract class PickAndPlaceBase : OperatorSkillBase
    {
        protected enum EPrimitives
        {
            /* OPEN_GRIPPER, */ PRE_GRASP, GRASP, /* CLOSE_GRIPPER, */ PICKUP, PRE_PLACE, PLACE, /* RELEASE */
        }
        private bool GetMappedInput(in object[] input, out Vector3 pickPosition, out Vector3 pickOrientation,
            out Vector3 placePosition, out Vector3 placeOrientation)
        {
            pickPosition = pickOrientation = placePosition = placeOrientation = Vector3.zero;
            if (input.Length < 4 || !input.All(x => x is Vector3))
                return false;
            pickPosition = (Vector3) input[0];
            pickOrientation = (Vector3) input[1];
            placePosition = (Vector3) input[2];
            placeOrientation = (Vector3) input[3];
            return true;
        }
        public override SkillResult Run(object[] inputs, object[] inOuts)
        {
            if (!GetMappedInput(inputs, out Vector3 pickPosition, out Vector3 pickOrientation, 
                out Vector3 placePosition, out Vector3 placeOrientation))
            {
                return SkillResult.Failure(TimeSpan.Zero, "Bad input");
            }
            return Run(pickPosition, pickOrientation, placePosition, placeOrientation);
        }
        public override async Task<SkillResult> RunAsync(object[] inputs, object[] inOuts)
        {
            if (!GetMappedInput(inputs, out Vector3 pickPosition, out Vector3 pickOrientation, 
                out Vector3 placePosition, out Vector3 placeOrientation))
            {
                return SkillResult.Failure(TimeSpan.Zero, "Bad input");
            }
            return await RunAsync(pickPosition, pickOrientation, placePosition, placeOrientation);
        }
        public abstract SkillResult Run(Vector3 pickPosition, Vector3 pickOrientation, 
            Vector3 placePosition, Vector3 placeOrientation);
        public abstract Task<SkillResult> RunAsync(Vector3 pickPosition, Vector3 pickOrientation, 
            Vector3 placePosition, Vector3 placeOrientation);
    }
}