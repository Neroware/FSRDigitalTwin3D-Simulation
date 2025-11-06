using FSR.DigitalTwin.Client.Common.Interfaces;
using FSR.DigitalTwin.Client.Features.DES;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using FSR.DigitalTwin.Client.Features.DES.Utils;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Common
{
    public class UnityVirtualWorkspace : MonoBehaviour, IVirtualWorkspace
    {
        [SerializeField] private SimulationManager simulationManager;
        [SerializeField] private HRCFunctionFactory functionFactory;

        public SimulationManager SimulationManager => simulationManager;
        public IHRCFunctionFactory FunctionFactory => functionFactory;

        private void Awake() => VirtualWorkspace.SetWorkspace(this);
    }
}