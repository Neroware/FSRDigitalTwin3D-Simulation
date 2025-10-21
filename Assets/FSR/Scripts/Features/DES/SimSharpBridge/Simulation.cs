using System;
using System.Threading;
using SimSharp;
using UniRx;

namespace FSR.DigitalTwin.Client.Features.DES.SimSharpBridge
{
    public class Simulation : PseudoRealtimeSimulation
    {
        private object _timeLocker = new();
        public override DateTime Now
        {
            get
            {
                lock (_timeLocker)
                {
                    if (!IsRunningInRealtime) return base.Now;
                    return base.Now + TimeSpan.FromMilliseconds(_rtDelayTime.Elapsed.TotalMilliseconds * RealtimeScale.Value);

                }
            }
            protected set => base.Now = value;
        }

        protected new Stopwatch _rtDelayTime = new();

        public Simulation() : this(new DateTime(1970, 1, 1)) { }
        public Simulation(TimeSpan? defaultStep) : this(new DateTime(1970, 1, 1), defaultStep) { }
        public Simulation(DateTime initialDateTime, TimeSpan? defaultStep = null) : this(new PcgRandom(), initialDateTime, defaultStep) { }
        public Simulation(int randomSeed, TimeSpan? defaultStep = null) : this(new DateTime(1970, 1, 1), randomSeed, defaultStep) { }
        public Simulation(DateTime initialDateTime, int randomSeed, TimeSpan? defaultStep = null) : this(new PcgRandom(randomSeed), initialDateTime, defaultStep) { }
        public Simulation(IRandom random, DateTime initialDateTime, TimeSpan? defaultStep = null) : base(random, initialDateTime, defaultStep) { }

        public override object Run(Event stopEvent = null)
        {
            _stop = new CancellationTokenSource();
            if (stopEvent != null)
            {
                if (stopEvent.IsProcessed)
                {
                    return stopEvent.Value;
                }
                stopEvent.AddCallback(StopSimulation);
            }
            OnRunStarted();

            var completedEvent = new ManualResetEventSlim(false);
            var stop = Observable.EveryUpdate()
                .Where(_ => ScheduleQ.Count == 0 || _stop.IsCancellationRequested)
                .First();
            var step = Observable.EveryUpdate().TakeUntil(stop);

            object[] error = new object[] { null };

            void onSimulationStop(StopSimulationException e)
            {
                OnRunFinished();
                completedEvent.Set();
                error[0] = e?.Value;
            }
            void onSimulationFinish()
            {
                OnRunFinished();
                completedEvent.Set();
            }
            void onStep()
            {
                try
                {
                    Step();
                }
                catch (StopSimulationException e)
                {
                    onSimulationStop(e);
                } 
            }
            var _ = step.Subscribe((_) => onStep(), (e) => onSimulationStop(e as StopSimulationException), () => onSimulationFinish());
            completedEvent.Wait();
            if (error[0] != null) return error[0];
            if (stopEvent == null) return null;
            if (!_stop.IsCancellationRequested && !stopEvent.IsTriggered) throw new InvalidOperationException("No scheduled events left but \"until\" event was not triggered.");
            return stopEvent.Value;
        }

        public override void Step()
        {
            var delay = TimeSpan.Zero;
            double? rtScale = null;
            lock (_locker)
            {
                if (IsRunningInRealtime)
                {
                    rtScale = RealtimeScale;
                    var next = ScheduleQ.First.PrimaryPriority;
                    delay = next - base.Now;
                    if (rtScale.Value != 1.0) delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds / rtScale.Value);
                    _rtDelayCtrl = CancellationTokenSource.CreateLinkedTokenSource(_stop.Token);
                }
            }

            if (delay > TimeSpan.Zero)
            {
                if (!_rtDelayTime.IsRunning) _rtDelayTime.Start();
                if (_rtDelayTime.Elapsed < delay)
                {
                    return;
                }
                _rtDelayTime.Stop();
                var observed = _rtDelayTime.Elapsed;

                lock (_locker)
                {
                    if (rtScale.Value != 1.0) observed = TimeSpan.FromMilliseconds(observed.TotalMilliseconds * rtScale.Value);
                    if (_rtDelayCtrl.IsCancellationRequested)
                    {
                        lock (_timeLocker)
                        {
                            Now = base.Now + observed;
                            _rtDelayTime.Reset();
                        }
                        return; // next event is not processed, step is not actually completed
                    }
                }
            }

            Event evt;
            lock (_locker) {
                var next = ScheduleQ.Dequeue();
                lock (_timeLocker) {
                    _rtDelayTime.Reset();
                    Now = next.PrimaryPriority;
                }
                evt = next.Event;
            }
            evt.Process();
            ProcessedEvents++;
        }

    }
};