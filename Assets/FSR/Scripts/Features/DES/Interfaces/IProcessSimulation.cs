using System;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming;
using FSR.DigitalTwin.Client.Features.UnityClient;

namespace FSR.DigitalTwin.Client.Features.DES.Interfaces
{
    public interface IProcessSimulation
    {
        IObservable<IProcessSimulation> SimulationStarted { get; }
        IObservable<IProcessSimulation> SimulationFinished { get; }
        IObservable<IProcessSimulation> SimulationReset { get; }

        IObservable<HRCProcess> ProcessStarted { get; }
        IObservable<HRCProcessResult> ProcessFinished { get; }
        IObservable<HRCProcess> ProcessFailed { get; }

        bool Initialize(out IProcessSimulationContext context);
        void Run();
        void Stop();
        void Reset();
        DateTime Now();

        void Process(HRCProcess process, IObservable<HRCProcessResult> success, IObservable<Exception> failure = null);

        bool IsRunning { get; }
        bool IsFinished { get; }
    }

    public interface IProcessSimulationContext
    {
        float Horizon { init; get; }
        IList<DigitalTwinActorBase> Actors { init; get; }
        IList<SocialOperatorBase> Operators { init; get; }
        IDictionary<HRCGoal, IList<HRCMethod>> Goals { init; get; }
        IDictionary<HRCMethod, IDictionary<HRCTask, IList<ISet<HRCTask>>>> Methods { init; get; }
        IList<HRCFunction> Functions { init; get; }
        IProcessSimulation Simulation { set; get; }

        /* TODO Later add simulation parameters as well... */
    }

}