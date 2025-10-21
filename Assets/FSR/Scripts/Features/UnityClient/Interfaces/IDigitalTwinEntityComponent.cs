using System;

namespace FSR.DigitalTwin.Client.Features.UnityClient.Interfaces {

    public interface IDigitalTwinEntityComponent {
        IDigitalTwinEntity DigitalTwinEntity { get; init; }
        Uri Id { get; init; }
        bool HasConnection { get; }
    }

}