using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill
{
    public abstract class PickAndPlace : OperatorSkillBase
    {
        private readonly MotionBase preGraspPose;
        private readonly MotionBase graspPose;
        private readonly MotionBase pickupPose;
        private readonly MotionBase prePlacePose;
        private readonly MotionBase placePose;
        // private readonly GripperActionBase openGripper;
        // private readonly GripperActionBase closeGripper;

        public override List<IDevicePrimitive> Primitives => new ()
        {
            // openGripper,
            preGraspPose,
            graspPose,
            // closeGripper,
            pickupPose,
            prePlacePose,
            placePose,
            // openGripper  
        };

        private enum EPrimitives
        {
            /* OPEN_GRIPPER, */ PRE_GRASP, GRASP, /* CLOSE_GRIPPER, */ PICKUP, PRE_PLACE, PLACE, /* RELEASE */
        }

        public override object[] MapPrimitiveInput(int primitive, object[] inputs, object[] inOuts)
        {
            return (EPrimitives) primitive switch
            {
                EPrimitives.PRE_GRASP or EPrimitives.GRASP or EPrimitives.PICKUP 
                    or EPrimitives.PRE_PLACE or EPrimitives.PLACE => inputs,
                // case ...
                //    return new object[0];
                _ => throw new IndexOutOfRangeException("should not happen"),
            };
        }
        public override void Execute(int primitive, object[] input, in SkillResult result)
        {
            switch ((EPrimitives) primitive)
            {
                case EPrimitives.PRE_GRASP: prePlacePose.Execute(input); break;
                case EPrimitives.GRASP: prePlacePose.Execute(input); break;
                case EPrimitives.PICKUP: prePlacePose.Execute(input); break;
                case EPrimitives.PRE_PLACE: prePlacePose.Execute(input); break;
                case EPrimitives.PLACE: prePlacePose.Execute(input); break;
                default:
                    throw new IndexOutOfRangeException("should not happen");
            }
        }
        public override async Task ExecuteAsync(int primitive, object[] input, SkillResult result)
        {
            switch ((EPrimitives) primitive)
            {
                case EPrimitives.PRE_GRASP: await prePlacePose.ExecuteAsync(input); break;
                case EPrimitives.GRASP: await prePlacePose.ExecuteAsync(input); break;
                case EPrimitives.PICKUP: await prePlacePose.ExecuteAsync(input); break;
                case EPrimitives.PRE_PLACE: await prePlacePose.ExecuteAsync(input); break;
                case EPrimitives.PLACE: await prePlacePose.ExecuteAsync(input); break;
                default:
                    throw new IndexOutOfRangeException("should not happen");
            }
        }

        // TODO Implement abstract base class

        // protected abstract Task<bool> ExecuteAsync(Transform pick, Vector3 pickDirection, Transform place, Vector3 placeDirection);
        // protected abstract bool Execute(Transform pick, Vector3 pickDirection, Transform place, Vector3 placeDirection);
    }
}