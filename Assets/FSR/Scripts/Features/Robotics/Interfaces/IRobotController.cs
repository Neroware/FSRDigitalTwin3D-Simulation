using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.Interfaces
{

    /// <summary>
    /// FSRDigitalTwin3D's common interface for all robot controllers, be it a ROS2-controller, 
    /// a game logic in Unity that describes movement or any other thing than can plan and run a movement.
    /// </summary>
    public interface IRobotController
    {
        GameObject Robot { get; }
        bool Plan();
        Task<bool> PlanAsync();
        bool ValidatePlan();
        void RunPlan();
        Task RunPlanAsync();
        void PlanAndRun();
        Task PlanAndRunAsync();
        bool Interrupt();
        void ForceInterrupt();
    }
}