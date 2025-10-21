using FSR.DigitalTwin.Client.Features.DES.Interfaces;

namespace FSR.DigitalTwin.Client.Features.UnityClient.Interfaces
{
    public interface IDigitalWorkspaceKnowledge
    {
        IProcessSimulationContext GetContext(float horizon = 86400.0f);
    }
}