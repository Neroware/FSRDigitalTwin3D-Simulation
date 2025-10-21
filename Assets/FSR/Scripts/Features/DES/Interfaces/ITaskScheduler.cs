

using System;

namespace FSR.DigitalTwin.Client.Features.DES.Interfaces
{
    public interface ITaskScheduler
    {
        IDisposable Schedule(ProcessSimulationBase sim, IProcessSimulationContext ctxt);
        IDisposable ScheduleGoal(HRCGoal goal, ProcessSimulationBase sim, IProcessSimulationContext ctxt);
        IDisposable ScheduleMethod(HRCMethod method, ProcessSimulationBase sim, IProcessSimulationContext ctxt);
    }
}