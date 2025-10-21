using System;
using System.Collections.Generic;
using System.Linq;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming;
using SimSharp;
using UniRx;
using UnityEngine;
using Event = SimSharp.Event;

namespace FSR.DigitalTwin.Client.Features.DES.SimSharpBridge
{
    public class SimSharpProcessSimulation : ProcessSimulationBase
    {
        private CompositeDisposable _disposable;
        private Simulation _environment;
        private Event _stopEvent;
        private IProcessSimulationContext _context;

        private double _rtTimeScale = 1.0;
        private bool _rtTimeSkips = true;
        private int _runningFunctionCounter = 0;

        public double TimeScale { set => _rtTimeScale = value; }
        public bool VirtualTimeSkips { set => _rtTimeSkips = value; }
        public Simulation Environment => _environment;

        private enum EState
        {
            AWAIT_INIT, INITIALIZED, RUNNING, TERMINATED
        }
        private EState _state = EState.AWAIT_INIT;
        public override bool IsRunning => _state == EState.RUNNING;
        public override bool IsFinished => _state == EState.TERMINATED;

        public SimSharpProcessSimulation(double rtTimeScale = 1.0, bool rtTimeSkips = true)
        {
            _rtTimeScale = rtTimeScale;
            _rtTimeSkips = rtTimeSkips;
        }

        public Process Process(IEnumerable<Event> generator, int priority = 0)
            => _environment.Process(generator, priority);

        protected override void OnInitialize(IProcessSimulationContext context)
        {
            if (_state == EState.RUNNING) throw new InvalidOperationException("Simulation already running. Use stop command first.");
            if (_state == EState.TERMINATED) throw new InvalidOperationException("Simulation already terminated. Use reset command first.");
            _state = EState.INITIALIZED;
            _environment = new Simulation();
            if (_rtTimeSkips) 
                _environment.SetVirtualtime();
            else
                _environment.SetRealtime(_rtTimeScale);
            _stopEvent = new(_environment);
            _context = context;
            _disposable = new();
        }

        protected override void OnReset()
        {
            if (_state == EState.AWAIT_INIT) throw new InvalidOperationException("Simulation not initialized.");
            if (_state == EState.RUNNING) throw new InvalidOperationException("Simulation still running. Use stop command first.");
            _state = EState.INITIALIZED;
            _disposable.Dispose();
            OnInitialize(_context);
        }

        protected override async void OnRun()
        {
            if (_state == EState.AWAIT_INIT) throw new InvalidOperationException("Simulation not initialized.");
            if (_state == EState.RUNNING) throw new InvalidOperationException("Simulation already running.");
            if (_state == EState.TERMINATED) throw new InvalidOperationException("Simulation already terminated. Use reset command first.");
            _state = EState.RUNNING;
            _disposable.Add(new NaiveTaskScheduler().Schedule(this, _context));
            _disposable.Add(
                Observable.Zip(_context.Goals.Keys
                    .Select(goal => _context.Simulation.ProcessFinished
                        .Where(p => (p.Process as HRCGoal)?.GoalId == goal.GoalId))
                    )
                .Subscribe(_ => _stopEvent.Trigger(_stopEvent))
            );
            _disposable.Add(_context.Simulation.ProcessFinished.Subscribe(p =>
            {
                Debug.Log($"Finished process: {p}");
            }));
            await _environment.RunAsync(_stopEvent);
            _state = EState.TERMINATED;
            Debug.Log("Finished process simulation run...");
        }

        protected override void OnStop()
        {
            if (_state != EState.RUNNING) throw new InvalidOperationException("Simulation not running.");
            _state = EState.TERMINATED;
            if (_stopEvent.IsTriggered)
                return;
            _stopEvent.Trigger(_stopEvent);
        }

        protected override void OnProcess(HRCProcess process, IObservable<HRCProcessResult> success, IObservable<Exception> failure)
        {
            Event p = new(_environment);
            Event timeout_ = _environment.Timeout(TimeSpan.FromDays(1));
            IEnumerable<Event> process_()
            {
                process.Timestamp = _environment.Now;
                _processStarted.OnNext(process);
                yield return new AnyOf(_environment, p, timeout_);
                _processFinished.OnNext(new HRCProcessResult()
                {
                    Process = process,
                    Succeeded = true,
                    TimeStamp = _environment.Now,
                    Outputs = new object[0]
                });
            }
            _disposable.Add(success.Subscribe(_ => { p.Trigger(p); }));
            _disposable.Add(failure.Subscribe(_ => { p.Fail(); _processFailed.OnNext(process); }));
            _environment.Process(process_());
        }

        protected override void OnProcess(HRCFunction function, bool realtime, IObservable<HRCProcessResult> success, IObservable<Exception> failure)
        {
            if (realtime)
            {
                OnProcess(function, success, failure);
            }
            else
            {
                Event p = new(_environment);
                Event timeout_ = _environment.Timeout(function?.FunctionDescription.Duration ?? TimeSpan.Zero);
                IEnumerable<Event> process_()
                {
                    function.Timestamp = _environment.Now;
                    _processStarted.OnNext(function);
                    yield return new AnyOf(_environment, p, timeout_);
                    _processFinished.OnNext(new HRCProcessResult()
                    {
                        Process = function,
                        Succeeded = true,
                        TimeStamp = _environment.Now,
                        Outputs = new object[0]
                    });
                }
                _disposable.Add(success.Subscribe(_ => { p.Trigger(p); }));
                _disposable.Add(failure.Subscribe(_ => { p.Fail(); _processFailed.OnNext(function); }));
                _environment.Process(process_());
            }
        }

        protected override bool OnFunctionLaunch(HRCFunction function, out SocialOperatorBase socialOperator)
        {
            socialOperator = null;
            var agent = _context.Operators
                .Where(op => op.AgentType == EHRCAgentType.Any
                    || function.FunctionDescription.AgentType == EHRCAgentType.Any 
                    || function.FunctionDescription.AgentType == op.AgentType)
                .Where(op => op.CanRun(new Uri(function.FunctionDescription.FunctionType)))
                .FirstOrDefault();
            if (agent == null)
            {
                return false;
            }
            _environment.SetRealtime(_rtTimeScale);
            if (_rtTimeSkips)
            {
                _runningFunctionCounter++;
                _disposable.Add(
                    ObserveOnTaskFinished<HRCFunction>(function.TaskId)
                        .Where(_ => --_runningFunctionCounter == 0)
                        .Subscribe(_ => _environment.SetVirtualtime()));
            }
            socialOperator = agent;
            return true;
        }

        public override DateTime Now() => _environment.Now;
    }

}