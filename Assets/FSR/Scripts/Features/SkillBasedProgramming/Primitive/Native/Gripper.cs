using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using UniRx;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive.Native
{
    public class Gripper : GripperBase
    {
        private readonly NativeGripperController _controller;
        private readonly EMode _mode;
        public enum EMode
        {
            OPEN, CLOSE
        }
        public Gripper(NativeGripperController controller, EMode mode)
        {
            _controller = controller;
            _mode = mode;
        }
        public override PrimitiveResult Execute()
        {
            if (_mode == EMode.OPEN)
            {
                _controller.OpenGripper();
                _controller.GripperOpened.First().ToTask().RunSynchronously();
            }
            else
            {
                _controller.CloseGripper();
                _controller.GripperClosed.First().ToTask().RunSynchronously();
            }
            return PrimitiveResult.Success(new object[0]);
        }
        public override async Task<PrimitiveResult> ExecuteAsync()
        {
            if (_mode == EMode.OPEN)
            {
                _controller.OpenGripper();
                await _controller.GripperOpened.First().ToTask();
            }
            else
            {
                _controller.CloseGripper();
                await _controller.GripperClosed.First().ToTask();
            }
            return PrimitiveResult.Success(new object[0]);
        }
    }
}