using FSR.DigitalTwin.Client.Features.Robotics.Interfaces;
using FSR.DigitalTwin.Client.Features.UnityClient;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.Controller
{

    public abstract class RobotControllerComponent : DigitalTwinComponentBase, IRobotController
    {
        public abstract GameObject Robot { get; }
        public abstract ReadOnlyReactiveProperty<bool> HasPlanned { get; }
        public abstract ReadOnlyReactiveProperty<bool> IsValid { get; }
        public abstract ReadOnlyReactiveProperty<bool> IsInterrupted { get; }
        public abstract ReadOnlyReactiveProperty<bool> IsRunning { get; }

        public abstract void ForceInterrupt();
        public abstract bool Interrupt();

        public abstract void Plan();
        public abstract void RunPlan();
        public abstract bool ValidatePlan();

        /// <summary>
        /// Plans and runs the plan if valid.
        /// </summary>
        public virtual void PlanAndRun()
        {
            if (!HasPlanned.Value) 
                Plan();
            HasPlanned
                .Where(x => x)
                .First()
                .Subscribe(_ =>
                {
                    if (ValidatePlan())
                    {
                        RunPlan();
                    }
                })
                .AddTo(this);
        }
    }
}