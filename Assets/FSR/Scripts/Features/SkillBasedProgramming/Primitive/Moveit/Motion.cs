using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive.Moveit
{
    public class Motion : MotionBase
    {
        private readonly RosMoveitController _controller;
        public Motion(RosMoveitController controller)
        {
            _controller = controller;
        }
        public override async Task<MotionResult> ExecuteAsync(string task, Vector3 target, Vector3 orientation)
        {
            _controller.Target = target;
            _controller.TargetOrientation = orientation;
            _controller.TrajectoryName = $"{Name}.{Base64Converter.EncodeString(task)}";
            await _controller.PlanAsync();
            if (!_controller.ValidatePlan())
            {
                return MotionResult.Failure();
            }
            await _controller.RunPlanAsync();
            return MotionResult.Success(new double[0]); // TODO Return joint orientation as output
        }
        public override MotionResult Execute(string task, Vector3 target, Vector3 orientation)
        {
            _controller.Target = target;
            _controller.TargetOrientation = orientation;
            _controller.TrajectoryName = task;
            _controller.Plan();
            if (!_controller.ValidatePlan())
            {
                return MotionResult.Failure();
            }
            _controller.RunPlan();
            return MotionResult.Success(new double[0]); // TODO Return joint orientation as output
        }
    }
}