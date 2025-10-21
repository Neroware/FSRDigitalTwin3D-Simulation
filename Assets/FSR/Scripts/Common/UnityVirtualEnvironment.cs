using FSR.DigitalTwin.Client.Common.Interfaces;
using FSR.DigitalTwin.Client.Features.DES;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Common
{
    public class UnityVirtualWorkspace : MonoBehaviour, IVirtualWorkspace
    {
        [SerializeField] private SimulationManager simulationManager;
        public SimulationManager SimulationManager => simulationManager;

        public UnityVirtualWorkspace()
        {
            VirtualWorkspace.SetWorkspace(this);
        }
    }
}