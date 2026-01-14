using FSR.DigitalTwin.Client.App.Config;
using FSR.DigitalTwin.Client.Common;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using FSR.DigitalTwin.Client.Features.UnityClient;
using UnityEngine;

namespace FSR.DigitalTwin.Client.App
{
    public class AppCallbacks : MonoBehaviour
    {
        public static void RunDESSimulation(string scenario)
        {
            var simulationManager = VirtualWorkspace.Instance.SimulationManager;
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
                var scenarioId = UriPrefix.PI + scenario;
                simulationManager.AddScenario(scenarioId);
                simulationManager.SetActiveScenario(scenarioId);
            }
            simulationManager.RunActiveScenario();
        }
        public static async void PickAndPlace(RosMoveitPickAndPlaceController controller)
        {
             await controller.PlanAndRunAsync();
        }
        public static async void RunPickAndPlaceSkill(PickAndPlaceConfig config)
        {
            config.pnp.PickOffset = config.pickOffset;
            config.pnp.PlaceOffset = config.placeOffset;
            await config.pnp.RunAsync(config.taskId, config.pickTarget.transform,
                config.pickOrientation, config.placeLocation.transform, config.placeOrientation);
        }
        public static async void RunScrewSkill(ScrewConfig config)
        {
            config.skill.EEOffset = config.eeOffset;
            config.skill.EEOrientation = config.eeOrientation;
            config.skill.StartPosition = config.startPosition;
            config.skill.ScrewOffset = config.screwOffset;
            await config.skill.RunAsync(config.taskId, config.target.transform, config.orientation);
        }
        public static async void RunTrajectory(RosMoveitController controller)
        {
            await controller.PlanAndRunAsync();
        }
    }
}