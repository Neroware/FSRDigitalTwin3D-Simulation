using UnityEngine;

namespace Unity.Robotics.UrdfImporter.Control {

public abstract class UrdfController : MonoBehaviour {
        public abstract float Stiffness { set; get; }
        public abstract float Damping { set; get; }
        public abstract float ForceLimit { set; get; }
        public abstract float Speed { set; get; }
        public abstract float Torque { set; get; }
        public abstract float Acceleration { set; get; }
        public abstract int DefDyanmicVal { set; get; }
        public abstract void UpdateControlType(JointControl joint);
}

} // Unity.Robotics.UrdfImporter.Control