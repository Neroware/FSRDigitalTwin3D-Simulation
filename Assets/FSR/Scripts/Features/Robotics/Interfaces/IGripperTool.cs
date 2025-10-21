namespace FSR.DigitalTwin.Client.Features.Robotics.Interfaces {

    public interface IGripperTool {

        bool Opened { get; }

        void OpenGripper();
        void CloseGripper();

    }

}