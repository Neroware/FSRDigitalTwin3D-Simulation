using System.Collections.Generic;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive.Native;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.Moveit
{
    public class PickAndPlace : PickAndPlaceBase
    {
        [SerializeField] private RosMoveitController _controller;
        [SerializeField] private NativeGripperController _gripperController;
        [SerializeField] private Vector3 prePickOffset = -0.2f * Vector3.up;
        [SerializeField] private Vector3 prePlaceOffset = -0.5f * Vector3.up;
        
        // Parameters
        public Vector3 PrePickOffset { set => prePickOffset = value; get => prePickOffset; }
        public Vector3 PrePlaceOffset { set => prePlaceOffset = value; get => prePlaceOffset; }

        protected override IEnumerable<IDevicePrimitive> GetPrimitivePlan(Transform pickTarget, Vector3 pickOrientation, Transform placeTarget, Vector3 placeOrientation)
        {
            Vector3 pickPosition = pickTarget.position + PickOffset;
            Vector3 placePosition = placeTarget.position + PlaceOffset;
            yield return new Primitive.Moveit.Motion(_controller) 
            { 
                Name = "pnp-pre-grasp",
                Target = pickPosition + prePickOffset,
                Orientation = pickOrientation
            };
            yield return new Gripper(_gripperController, Gripper.EMode.OPEN) { Name = "pnp-open-gripper" };
            yield return new Primitive.Moveit.Motion(_controller) 
            { 
                Name = "pnp-grasp",
                Target = pickPosition,
                Orientation = pickOrientation
            };
            yield return new Gripper(_gripperController, Gripper.EMode.CLOSE) { Name = "pnp-close-gripper" };
            yield return new Primitive.Moveit.Motion(_controller) 
            { 
                Name = "pnp-pickup",
                Target = pickPosition + prePickOffset,
                Orientation = pickOrientation
            };
            yield return new Primitive.Moveit.Motion(_controller) 
            { 
                Name = "pnp-pre-place",
                Target = placePosition + prePlaceOffset,
                Orientation = pickOrientation
            };
            yield return new Primitive.Moveit.Motion(_controller) 
            { 
                Name = "pnp-place",
                Target = placePosition,
                Orientation = pickOrientation
            };
            yield return new Gripper(_gripperController, Gripper.EMode.OPEN) { Name = "pnp-release" };
        }
    }
}