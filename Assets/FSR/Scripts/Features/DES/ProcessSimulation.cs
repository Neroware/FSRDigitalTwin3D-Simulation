using System;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming;
using FSR.DigitalTwin.Client.Features.UnityClient;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.DES
{
    public class ProcessSimulationContext : IProcessSimulationContext
    {
        public float Horizon { get; init; }
        public IList<DigitalTwinActorBase> Actors { get; init; } = new List<DigitalTwinActorBase>();
        public IList<SocialOperatorBase> Operators { get; init; } = new List<SocialOperatorBase>();
        public IDictionary<HRCGoal, IList<HRCMethod>> Goals { get; init; } = new Dictionary<HRCGoal, IList<HRCMethod>>();
        public IDictionary<HRCMethod, IDictionary<HRCTask, IList<ISet<HRCTask>>>> Methods { get; init; } = new Dictionary<HRCMethod, IDictionary<HRCTask, IList<ISet<HRCTask>>>>();
        public IList<HRCFunction> Functions { get; init; } = new List<HRCFunction>();
        public IProcessSimulation Simulation { get; set; }
    }

    public abstract class ProcessSimulationBase : IProcessSimulation
    {
        public IObservable<IProcessSimulation> SimulationStarted => _simulationStarted;
        public IObservable<IProcessSimulation> SimulationFinished => _simulationFinished;
        public IObservable<IProcessSimulation> SimulationReset => _simulationReset;
        public IObservable<HRCProcess> ProcessStarted => _processStarted;
        public IObservable<HRCProcessResult> ProcessFinished => _processFinished;
        public IObservable<HRCProcess> ProcessFailed => _processFailed;

        public abstract bool IsRunning { get; }
        public abstract bool IsFinished { get; }

        protected Subject<IProcessSimulation> _simulationStarted = new();
        protected Subject<IProcessSimulation> _simulationFinished = new();
        protected Subject<IProcessSimulation> _simulationReset = new();
        protected Subject<HRCProcess> _processStarted = new();
        protected Subject<HRCProcessResult> _processFinished = new();
        protected Subject<HRCProcess> _processFailed = new();

        public IObservable<HRCProcessResult<ProcessT>> ObserveOnTaskFinished<ProcessT>(string taskId) where ProcessT : HRCTask
        {
            return ProcessFinished
                .Where(p => (p.Process as HRCTask)?.TaskId == taskId)
                .First()
                .Select(p => new HRCProcessResult<ProcessT>()
                {
                    Process = p.Process as ProcessT,
                    Succeeded = p.Succeeded,
                    TimeStamp = p.TimeStamp,
                    Outputs = p.Outputs
                });
        }
        public IObservable<HRCProcessResult<HRCGoal>> ObserveOnGoalFinished(string goalId)
        {
            return ProcessFinished
                .Where(p => (p.Process as HRCGoal)?.GoalId == goalId)
                .First()
                .Select(p => new HRCProcessResult<HRCGoal>()
                {
                    Process = p.Process as HRCGoal,
                    Succeeded = p.Succeeded,
                    TimeStamp = p.TimeStamp,
                    Outputs = p.Outputs
                });
        }

        public bool Initialize(out IProcessSimulationContext context)
        {
            try
            {
                context = DigitalWorkspace.Instance.Knowledge.GetContext();
                context.Simulation = this;
                OnInitialize(context);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                context = null;
                return false;
            }
        }

        public void Process(HRCProcess process, IObservable<HRCProcessResult> success_ = null, IObservable<Exception> failure_ = null)
        {
            IObservable<HRCProcessResult> success = success_ ?? Observable.Never<HRCProcessResult>();
            IObservable<Exception> failure = failure_ ?? Observable.Never<Exception>();
            if (process is HRCFunction function)
            {
                bool hasOperator = OnFunctionLaunch(function, out SocialOperatorBase socialOperator);
                if (hasOperator)
                {
                    success = success.Merge(
                        socialOperator.RunFunctionAsync(function)
                            .ToObservable()
                            .Select(result => (HRCProcessResult)result)
                    );
                    OnProcess(function, true, success, failure);
                }
                else
                {
                    OnProcess(function, false, success, failure);
                }
            }
            else
            {
                OnProcess(process, success, failure);
            }
        }

        public void Reset()
        {
            OnReset();
            _simulationReset.OnNext(this);
        }
        public void Run()
        {
            OnRun();
            _simulationStarted.OnNext(this);
        }
        public void Stop()
        {
            OnStop();
            _simulationFinished.OnNext(this);
        }

        protected abstract void OnStop();
        protected abstract void OnRun();
        protected abstract void OnReset();
        protected virtual bool OnFunctionLaunch(HRCFunction function, out SocialOperatorBase socialOperator)
        {
            socialOperator = null;
            return false;
        }
        protected abstract void OnInitialize(IProcessSimulationContext context);
        protected virtual void OnProcess(HRCProcess process, IObservable<HRCProcessResult> success, IObservable<Exception> failure) => _processStarted.OnNext(process);
        protected virtual void OnProcess(HRCFunction function, bool realtime, IObservable<HRCProcessResult> success, IObservable<Exception> failure) => OnProcess(function, success, failure);
        public abstract DateTime Now();

    }

}