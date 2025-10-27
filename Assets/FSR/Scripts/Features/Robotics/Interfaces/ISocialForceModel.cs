using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.Interfaces
{
    public interface ISocialForceModel
    { 
        Transform Goal { set; get; }
        Vector3 GoalForce { get; }
        Vector3 RepulsiveForce { get; }
        Vector3 TotalForce { get; }
    }
}