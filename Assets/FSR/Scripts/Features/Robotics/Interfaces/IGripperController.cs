using System;

namespace FSR.DigitalTwin.Client.Features.Robotics.Interfaces
{
    public interface IGripperController
    {
        IGripperTool Gripper { get; }
        IObservable<IGripperTool> GripperOpened { get; }
        IObservable<IGripperTool> GripperClosed { get; }
        public void OpenGripper();
        public void CloseGripper();

    }
}