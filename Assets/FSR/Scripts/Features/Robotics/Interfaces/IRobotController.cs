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
        ReadOnlyReactiveProperty<bool> HasPlanned { get; }
        ReadOnlyReactiveProperty<bool> IsValid { get; }
        ReadOnlyReactiveProperty<bool> IsInterrupted { get; }
        ReadOnlyReactiveProperty<bool> IsRunning { get; }

        void Plan();
        bool ValidatePlan();
        void RunPlan();
        // void PlanAndRunIfValid();
        bool Interrupt();
        void ForceInterrupt();
    }

    public interface IRobotAsyncController : IRobotController
    {
        Task PlanAsync();
        Task<bool> ValidatePlanAsync();
        Task RunPlanAsync();
        Task PlanAndRunIfValidAsync();
        Task<bool> InterruptAsync();
        Task ForceInterruptAsync();
    }

}