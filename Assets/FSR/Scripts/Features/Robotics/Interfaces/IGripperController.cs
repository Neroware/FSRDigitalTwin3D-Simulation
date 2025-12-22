using System;
using System.Threading.Tasks;

namespace FSR.DigitalTwin.Client.Features.Robotics.Interfaces
{
    public interface IGripperController
    {
        IGripperTool Gripper { get; }
        public void OpenGripper();
        public Task OpenGripperAsync();
        public void CloseGripper();
        public Task CloseGripperAsync();
    }
}