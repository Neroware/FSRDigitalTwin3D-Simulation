using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;

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
            }
            else
            {
                _controller.CloseGripper();
            }
            return PrimitiveResult.Success(new object[0]);
        }
        public override async Task<PrimitiveResult> ExecuteAsync()
        {
            if (_mode == EMode.OPEN)
            {
                await _controller.OpenGripperAsync();
            }
            else
            {
                await _controller.CloseGripperAsync();
            }
            return PrimitiveResult.Success(new object[0]);
        }
    }
}