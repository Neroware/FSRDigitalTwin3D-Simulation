using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.Robotics.Interfaces {

    public interface IRosSourceDestinationPublisher {

        string TopicName { get; }
        GameObject Robot { get; }
        GameObject Target { get; }
        GameObject TargetPlacement { get; }

        public void Publish();

    }

}