using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive.Moveit
{
    public class Motion : MotionBase
    {
        public float SpeedScale { set; get; } = 1.0f;
        private readonly RosMoveitController _controller;
        public Motion(RosMoveitController controller)
        {
            _controller = controller;
        }
        public override async Task<PrimitiveResult> ExecuteAsync(string task)
        {
            _controller.Target = Target;
            _controller.TargetOrientation = Orientation;
            _controller.TrajectoryName = $"{Base64Converter.EncodeString(Name)}.{Base64Converter.EncodeString(task)}";
            _controller.JointAssignmentLerpScale = (int)(1.0f / SpeedScale);
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
            _controller.JointAssignmentLerpScale = (int)(1.0f / SpeedScale);
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