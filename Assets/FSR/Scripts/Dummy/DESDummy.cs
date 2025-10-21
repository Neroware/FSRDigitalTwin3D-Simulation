using FSR.DigitalTwin.Client.Common.Utils.Semantic;
using FSR.DigitalTwin.Client.Features.DES;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using FSR.DigitalTwin.Client.Features.UnityClient;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Dummy
{
    public class DESDummy : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulationManager;

        public void RunDESSimulation()
        {
            if (simulationManager == null)
            {
                Debug.LogError("Missing reference to SimulationManager!");
                return;
            }
            if (!DigitalWorkspace.Instance.Connection.IsConnected.Value)
            {
                Debug.LogError("Missing connection to digital twin server!");
                return;
            }
            if (!simulationManager.HasActiveScenario())
            {
                var scenario = UriPrefix.PI + "myscenario1";
                simulationManager.AddScenario(scenario);
                simulationManager.SetActiveScenario(scenario);
            }
            simulationManager.RunActiveScenario();
        }
    }
}