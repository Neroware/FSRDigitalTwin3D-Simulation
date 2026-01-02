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
        public override async Task<PrimitiveResult> ExecuteAsync(string task)
        {
            _controller.Target = Target;
            _controller.TargetOrientation = Orientation;
            _controller.TrajectoryName = $"{Name}.{Base64Converter.EncodeString(task)}";
            await _controller.PlanAsync();
            if (!_controller.ValidatePlan())
            {
                return PrimitiveResult.Failure();
            }
            await _controller.RunPlanAsync();
            return PrimitiveResult.Success(new object[0]); // TODO Return joint orientation as output
        }
        public override PrimitiveResult Execute(string task)
        {
            _controller.Target = Target;
            _controller.TargetOrientation = Orientation;
            _controller.TrajectoryName = task;
            _controller.Plan();
            if (!_controller.ValidatePlan())
            {
                return PrimitiveResult.Failure();
            }
            _controller.RunPlan();
            return PrimitiveResult.Success(new object[0]); // TODO Return joint orientation as output
        }
    }
}