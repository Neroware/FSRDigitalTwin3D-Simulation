using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill
{
    public abstract class PickAndPlaceBase : OperatorSkillBase
    {
        [SerializeField] protected Vector3 pickOffset = Vector3.zero;
        [SerializeField] protected Vector3 placeOffset = Vector3.zero;
        protected enum EPrimitives
        {
            OPEN_GRIPPER, PRE_GRASP, GRASP, CLOSE_GRIPPER, PICKUP, PRE_PLACE, PLACE, RELEASE
        }
        public void SetOffsets(Vector3 pickOffset, Vector3 placeOffset)
        {
            this.pickOffset = pickOffset;
            this.placeOffset = placeOffset;
        }
        private bool GetMappedInput(in object[] input, out Transform pickTarget, out Vector3 pickOrientation,
            out Transform placeTarget, out Vector3 placeOrientation)
        {
            pickTarget = placeTarget = null;
            pickOrientation = placeOrientation = Vector3.zero;
            if (input.Length < 4 || input[0] is not Transform || input[1] is not Vector3
                || input[2] is not Transform || input[4] is not Vector3)
                    return false;
            pickTarget = (Transform) input[0];
            pickOrientation = (Vector3) input[1];
            placeTarget = (Transform) input[2];
            placeOrientation = (Vector3) input[3];
            return true;
        }
        public override SkillResult Run(string task, object[] inputs, object[] inOuts)
        {
            if (!GetMappedInput(inputs, out Transform pickPosition, out Vector3 pickOrientation, 
                out Transform placePosition, out Vector3 placeOrientation))
            {
                return SkillResult.Failure(TimeSpan.Zero, "Bad input");
            }
            return Run(task, pickPosition, pickOrientation, placePosition, placeOrientation);
        }
        public override async Task<SkillResult> RunAsync(string task, object[] inputs, object[] inOuts)
        {
            if (!GetMappedInput(inputs, out Transform pickPosition, out Vector3 pickOrientation, 
                out Transform placePosition, out Vector3 placeOrientation))
            {
                return SkillResult.Failure(TimeSpan.Zero, "Bad input");
            }
            return await RunAsync(task, pickPosition, pickOrientation, placePosition, placeOrientation);
        }
        public abstract SkillResult Run(string task, Transform pickTarget, Vector3 pickOrientation, 
            Transform placeTarget, Vector3 placeOrientation);
        public abstract Task<SkillResult> RunAsync(string task, Transform pickTarget, Vector3 pickOrientation, 
            Transform placeTarget, Vector3 placeOrientation);
    }
}