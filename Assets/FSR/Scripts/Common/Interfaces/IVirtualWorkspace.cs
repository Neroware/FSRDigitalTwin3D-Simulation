using FSR.DigitalTwin.Client.Features.DES;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;

namespace FSR.DigitalTwin.Client.Common.Interfaces {

    public interface IVirtualWorkspace
    {
        SimulationManager SimulationManager { get; }
        IHRCFunctionFactory FunctionFactory { get; }

    }

}