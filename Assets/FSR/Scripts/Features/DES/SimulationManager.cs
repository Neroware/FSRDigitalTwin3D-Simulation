using System;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Common.Utils.Semantic;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using FSR.DigitalTwin.Client.Features.DES.SimSharpBridge;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.DES
{
    public class SimulationManager : MonoBehaviour
    {
        [SerializeField] private double realTimeScale = 1.0f;
        [SerializeField] private bool virtualTimeSkipsEnabled = true;
        private Dictionary<Uri, IProcessSimulation> _scenarios = new();
        private Uri _activeScenario = null;

        Dictionary<Uri, IProcessSimulation> Scenarios => _scenarios;
        public IProcessSimulation ActiveScenario => _scenarios.ContainsKey(_activeScenario) ?
            _scenarios[_activeScenario] : throw new NullReferenceException("missing scenario, did you forget to add it?");
        public bool HasActiveScenario() => _activeScenario != null;
        public IProcessSimulation AddScenario(Uri scenario)
        {
            SimSharpProcessSimulation processSimulation = new(realTimeScale, virtualTimeSkipsEnabled);
            _scenarios.Add(scenario, processSimulation);
            return processSimulation;
        }
        public IProcessSimulation AddScenario(string scenario) => AddScenario(UriPrefix.PI + scenario);
        public void SetActiveScenario(Uri scenario)
        {
            if (HasActiveScenario() && ActiveScenario.IsRunning)
                throw new InvalidOperationException("Cannot update active scenario during simulation run.");
            _activeScenario = scenario;
        }
        public void SetActiveScenario(string scenario) => SetActiveScenario(UriPrefix.PI + scenario);
        public void RunActiveScenario()
        {
            var activeScenario = (SimSharpProcessSimulation)ActiveScenario;
            if (activeScenario.IsRunning) throw new InvalidOperationException("Active scenario already running.");
            activeScenario.TimeScale = realTimeScale;
            activeScenario.VirtualTimeSkips = virtualTimeSkipsEnabled;
            if (activeScenario.IsFinished)
            {
                activeScenario.Reset();
                activeScenario.Run();
            }
            else
            {
                if (activeScenario.Initialize(out IProcessSimulationContext _))
                    activeScenario.Run();
                else throw new InvalidOperationException("Failed to initialize scenario.");
            }
        }
        public void StopActiveScenario() => throw new NotImplementedException("Not implemented as it also requires stopping running operators.");
    }

}