using System;
using UniRx;

namespace FSR.DigitalTwin.Client.Features.Robotics.Interfaces {
    
    /// <summary>
    /// A sensor source within the Virtual Workspace. Generates observable data.
    /// </summary>
    public interface ISensorSource<T> {

        ReadOnlyReactiveProperty<T> SensorData { get; }

    }

}