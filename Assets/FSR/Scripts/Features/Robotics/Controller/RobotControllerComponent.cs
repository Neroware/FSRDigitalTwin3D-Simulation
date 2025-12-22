using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.Robotics.Interfaces;
using FSR.DigitalTwin.Client.Features.UnityClient;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.Controller
{

    public abstract class RobotControllerComponent : DigitalTwinComponentBase, IRobotController
    {
        public abstract GameObject Robot { get; }
        public abstract bool Plan();
        public abstract Task<bool> PlanAsync();
        public abstract bool ValidatePlan();
        public abstract void RunPlan();
        public abstract Task RunPlanAsync();
        public abstract bool Interrupt();
        public abstract void ForceInterrupt();

        /// <summary>
        /// Plans and runs the plan if valid.
        /// </summary>
        public virtual void PlanAndRun()
        {
            Plan();
            if (ValidatePlan())
            {
                RunPlan();
            }
        }
        /// <summary>
        /// Plans and runs the plan asynchronously if valid.
        /// </summary>
        public virtual async Task PlanAndRunAsync()
        {
            await PlanAsync();
            if (ValidatePlan())
            {
                await RunPlanAsync();
            }
        }
    }
}