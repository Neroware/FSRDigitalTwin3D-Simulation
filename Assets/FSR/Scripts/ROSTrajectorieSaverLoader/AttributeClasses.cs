// CacheTypes.cs
// DTOs / Attribute und Extraktions-Hilfsfunktionen
// Benötigt: RosMessageTypes.Ur5eMoveit, RosMessageTypes.Geometry

using System;
using System.Collections.Generic;
using RosMessageTypes.Ur5eMoveit;
using RosMessageTypes.Geometry;

[Serializable]
public class CacheFile
{
    //public RequestData request;
    public string filename;
    public ResponseData response;
}

// [Serializable]
// public class RequestData
// {
//     public Joints joints_input;
//     public PoseData pick_pose;
//     public PoseData place_pose;

//     public static implicit operator RequestData(bool v)
//     {
//         throw new NotImplementedException();
//     }
// }

// [Serializable]
// public class Joints
// {
//     public double[] joints_input;
//     public PoseData pick_pose;
//     public PoseData place_pose;
// }

// [Serializable]
// public class PoseData
// {
//     public double[] position;        // [x,y,z]
//     public double[] orientation;     // [x,y,z,w]
// }

[Serializable]
public class ResponseData
{
    public TrajectoryData[] trajectories;
}

[Serializable]
public class TrajectoryData
{
    public JointTrajectory joint_trajectory;
}

[Serializable]
public class JointTrajectory
{
    public JointTrajectoryPoints[] points;
}

[Serializable]
public class JointTrajectoryPoints
{
    public double[] positions;
}
