namespace FSR.DigitalTwin.Client.Features.Robotics.Interfaces {

    public interface IScrewerTool 
    {
        void PrepareScrew();
        void SetScrewPercentComplete(float percent);
        void ReleaseScrew();
    }

}