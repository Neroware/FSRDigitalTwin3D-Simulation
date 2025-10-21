using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.DES.SimSharpBridge;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Dummy
{
    public class SimSharpDummy : MonoBehaviour
    {
        private SimSharp.Simulation _simulation = null;

        private async void Start()
        {
            var sim = new Simulation(TimeSpan.FromSeconds(10));
            _simulation = sim;
            _simulation.Process(MyProcess());
            await sim.RunAsync();
            Debug.Log("DONE");
            // var _ = sim.RunAsync();
            // await _simulation.RunAs();
        }

        IEnumerable<SimSharp.Event> MyProcess()
        {
            Debug.Log("Hello from random process!");
            yield return _simulation.Timeout(TimeSpan.FromSeconds(5));
            Debug.Log("Random process done!");
        }
    }
}