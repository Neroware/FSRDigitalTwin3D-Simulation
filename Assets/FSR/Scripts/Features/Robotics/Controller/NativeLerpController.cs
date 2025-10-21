using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.Controller
{
    /// <summary>
    /// A native robot controller that uses linear interpolation between fixed joint positions
    /// </summary>
    public class NativeLerpController : RobotControllerComponent
    {
        // NativeLerpController variables
        [SerializeField] private float jointAssignmentWait = 0.1f;
        [SerializeField] private ArticulationBody[] jointArticulationBodies;
        

        // Controller Interface
        public override GameObject Robot => throw new System.NotImplementedException();
        public override ReadOnlyReactiveProperty<bool> HasPlanned => throw new System.NotImplementedException();
        public override ReadOnlyReactiveProperty<bool> IsValid => throw new System.NotImplementedException();
        public override ReadOnlyReactiveProperty<bool> IsInterrupted => throw new System.NotImplementedException();
        public override ReadOnlyReactiveProperty<bool> IsRunning => throw new System.NotImplementedException();

        public override void ForceInterrupt()
        {
            throw new System.NotImplementedException();
        }

        public override bool Interrupt()
        {
            throw new System.NotImplementedException();
        }

        public override void Plan()
        {
            throw new System.NotImplementedException();
        }

        public override void RunPlan()
        {
            throw new System.NotImplementedException();
        }

        public override bool ValidatePlan()
        {
            throw new System.NotImplementedException();
        }
    }
}