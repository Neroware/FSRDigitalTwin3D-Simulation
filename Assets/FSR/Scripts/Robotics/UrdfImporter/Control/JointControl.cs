// =================================================================================================================================== //
// Modified version of https://github.com/louis1218/digitaltwins-Unity/blob/main/Assets/ZeroMQ/Controller/handwave/JointControl.cs
// by louis1218
//
// For more information visit https://github.com/louis1218/digitaltwins-Unity
// =================================================================================================================================== //

using UnityEngine;

namespace Unity.Robotics.UrdfImporter.Control {
public class JointControl : MonoBehaviour
{
    UrdfController controller;

    public RotationDirection direction;
    public ControlType controltype;
    public float speed ;
    public float torque ;
    public float acceleration;
    public ArticulationBody joint;


    private void Start()
    {
        direction = 0;
        controller = (UrdfController)this.GetComponentInParent(typeof(UrdfController));
        joint = this.GetComponent<ArticulationBody>();
        controller.UpdateControlType(this);
        speed = controller.Speed;
        torque = controller.Torque;
        acceleration = controller.Acceleration;
    }

    private void FixedUpdate(){

        speed = controller.Speed;
        torque = controller.Torque;
        acceleration = controller.Acceleration;


        if (joint.jointType != ArticulationJointType.FixedJoint)
        {
            if (controltype == ControlType.PositionControl)
            {
                ArticulationDrive currentDrive = joint.xDrive;
                float newTargetDelta = (int)direction * Time.fixedDeltaTime * speed;

                if (joint.jointType == ArticulationJointType.RevoluteJoint)
                {
                    if (joint.twistLock == ArticulationDofLock.LimitedMotion)
                    {
                        if (newTargetDelta + currentDrive.target > currentDrive.upperLimit)
                        {
                            currentDrive.target = currentDrive.upperLimit;
                        }
                        else if (newTargetDelta + currentDrive.target < currentDrive.lowerLimit)
                        {
                            currentDrive.target = currentDrive.lowerLimit;
                        }
                        else
                        {
                            currentDrive.target += newTargetDelta;
                        }
                    }
                    else
                    {
                        currentDrive.target += newTargetDelta;
   
                    }
                }

                else if (joint.jointType == ArticulationJointType.PrismaticJoint)
                {
                    if (joint.linearLockX == ArticulationDofLock.LimitedMotion)
                    {
                        if (newTargetDelta + currentDrive.target > currentDrive.upperLimit)
                        {
                            currentDrive.target = currentDrive.upperLimit;
                        }
                        else if (newTargetDelta + currentDrive.target < currentDrive.lowerLimit)
                        {
                            currentDrive.target = currentDrive.lowerLimit;
                        }
                        else
                        {
                            currentDrive.target += newTargetDelta;
                        }
                    }
                    else
                    {
                        currentDrive.target += newTargetDelta;
   
                    }
                }
                joint.xDrive = currentDrive;
            }
        }
    }
}

} // Unity.Robotics.UrdfImporter.Control