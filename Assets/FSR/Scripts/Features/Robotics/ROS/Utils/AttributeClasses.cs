using System;

namespace FSR.DigitalTwin.Client.Features.Robotics.ROS.Utils
{

[Serializable]
public class CacheFile
{
    //public RequestData request;
    public string filename;
    public ResponseData response;
}

[Serializable]
public record ResponseData
{
    public TrajectoryData[] trajectories;
}

[Serializable]
public record TrajectoryData
{
    public JointTrajectory jointTrajectory;
}

[Serializable]
public record JointTrajectory
{
    public JointTrajectoryPoints[] points;
}

[Serializable]
public record JointTrajectoryPoints
{
    public double[] positions;
}

} // END FSR.DigitalTwin.Client.Features.Robotics.ROS.Utils