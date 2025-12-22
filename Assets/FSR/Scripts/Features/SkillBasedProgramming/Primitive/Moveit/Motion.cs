using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using UniRx;
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
        public override async Task<MotionResult> ExecuteAsync(Vector3 target, Vector3 orientation)
        {
            _controller.Target = target;
            _controller.TargetOrientation = orientation;
            _controller.TrajectoryName = Name;
            _controller.Plan();
            await _controller.HasPlanned.First(x => x).ToTask();
            if (!_controller.ValidatePlan())
            {
                return MotionResult.Failure();
            }
            _controller.RunPlan();
            await _controller.IsRunning.First(x => !x).ToTask();
            return MotionResult.Success(new double[0]); // TODO Return joint orientation as output
        }
        public override MotionResult Execute(Vector3 target, Vector3 orientation)
        {
            _controller.Target = target;
            _controller.TargetOrientation = orientation;
            _controller.TrajectoryName = Name;
            _controller.Plan();
            _controller.HasPlanned.First(x => x).ToTask().RunSynchronously();
            if (!_controller.ValidatePlan())
            {
                return MotionResult.Failure();
            }
            _controller.RunPlan();
            _controller.IsRunning.First(x => !x).ToTask().RunSynchronously();
            return MotionResult.Success(new double[0]); // TODO Return joint orientation as output
        }
    }
}