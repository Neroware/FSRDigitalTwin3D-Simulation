

using System;
using UniRx;

namespace FSR.DigitalTwin.Client.Features.DES.Interfaces
{
    public interface ITaskScheduler
    {
        IDisposable Schedule(ProcessSimulationBase sim, IProcessSimulationContext context, IObservable<Unit> previous = null);
        IDisposable Schedule(HRCGoal goal, ProcessSimulationBase sim, IProcessSimulationContext context, IObservable<Unit> previous = null);
        IDisposable Schedule(HRCMethod method, ProcessSimulationBase sim, IProcessSimulationContext context, IObservable<Unit> previous = null);
        IDisposable Schedule(HRCTask task, HRCMethod method, ProcessSimulationBase sim, IProcessSimulationContext context, IObservable<Unit> previous = null);
        IDisposable Schedule(HRCFunction function, ProcessSimulationBase sim, IProcessSimulationContext context, IObservable<Unit> previous = null);
    }
}