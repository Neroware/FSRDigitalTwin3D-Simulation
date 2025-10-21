using System;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.DES.SimSharpBridge
{
    public class Stopwatch
    {
        public TimeSpan Elapsed { get; private set; }
        public long ElapsedMilliseconds => Elapsed.Milliseconds;
        public bool IsRunning => _timer != null;
        private IDisposable _timer = null;

        public static Stopwatch StartNew()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            return stopwatch;
        }

        public void Reset()
        {
            if (IsRunning) Stop();
            Elapsed = TimeSpan.Zero;
        }
        public void Restart()
        {
            Reset();
            Start();
        }
        public void Start()
        {
            Elapsed = TimeSpan.Zero;
            _timer = Observable.EveryUpdate().Subscribe(_ => Elapsed += TimeSpan.FromSeconds(Time.deltaTime));
        }
        public void Stop()
        {
            _timer?.Dispose();
            _timer = null;
        }
    }
}