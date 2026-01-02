using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill
{
    public abstract class PickAndPlaceBase : OperatorSkillBase
    {
        [SerializeField] private Vector3 pickOffset = Vector3.zero;
        [SerializeField] private Vector3 placeOffset = Vector3.zero;
        // Parameters
        public Vector3 PickOffset { set => pickOffset = value; get => pickOffset; }
        public Vector3 PlaceOffset { set => placeOffset = value; get => placeOffset; }
        public enum EPrimitives
        {
            OPEN_GRIPPER, PRE_GRASP, GRASP, CLOSE_GRIPPER, PICKUP, PRE_PLACE, PLACE, RELEASE
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
        protected override IEnumerable<IDevicePrimitive> GetPrimitivePlan(object[] inputs, object[] inOuts)
        {
            if (!GetMappedInput(inputs, out Transform pickPosition, out Vector3 pickOrientation, 
                out Transform placePosition, out Vector3 placeOrientation))
            {
                Debug.LogError("Bad input! Failed to construct skill primitive plan!");
                return new IDevicePrimitive[0];
            }
            return GetPrimitivePlan(pickPosition, pickOrientation, placePosition, placeOrientation);
        }
        protected abstract IEnumerable<IDevicePrimitive> GetPrimitivePlan(Transform pickTarget, Vector3 pickOrientation, Transform placeTarget, Vector3 placeOrientation);
        public SkillResult Run(string task, Transform pickTarget, Vector3 pickOrientation, Transform placeTarget, Vector3 placeOrientation)
        {
            List<IDevicePrimitive> primitives = GetPrimitivePlan(
                pickTarget, pickOrientation, placeTarget, placeOrientation).ToList();
            return OnRun(task, primitives);
        }
        public Task<SkillResult> RunAsync(string task, Transform pickTarget, Vector3 pickOrientation, Transform placeTarget, Vector3 placeOrientation)
        {
            List<IDevicePrimitive> primitives = GetPrimitivePlan(
                pickTarget, pickOrientation, placeTarget, placeOrientation).ToList();
            return OnRunAsync(task, primitives);
        }
    }
}