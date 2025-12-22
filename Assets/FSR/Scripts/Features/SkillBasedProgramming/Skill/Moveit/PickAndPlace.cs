using System.Collections.Generic;
using System.Threading.Tasks;
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
        public override List<IDevicePrimitive> Primitives => new()
        {
            _preGrasp, _openGripper, _grasp, _closeGripper, 
            _pickup, _prePlace, _place, _release
        };
        private Primitive.Moveit.Motion _preGrasp, _grasp, _pickup, _prePlace, _place;
        private Gripper _openGripper, _closeGripper, _release;
        private void Start()
        {
            _preGrasp = new Primitive.Moveit.Motion(_controller) { Name = "pnp-pre-grasp" };
            _openGripper = new Gripper(_gripperController, Gripper.EMode.OPEN) { Name = "pnp-open-gripper" };
            _grasp = new Primitive.Moveit.Motion(_controller) { Name = "pnp-grasp" };
            _closeGripper = new Gripper(_gripperController, Gripper.EMode.CLOSE) { Name = $"pnp-close-gripper" };
            _pickup = new Primitive.Moveit.Motion(_controller) { Name = "pnp-pickup" };
            _prePlace = new Primitive.Moveit.Motion(_controller) { Name = "pnp-pre-place" };
            _place = new Primitive.Moveit.Motion(_controller) { Name = $"pnp-place" };
            _release = new Gripper(_gripperController, Gripper.EMode.OPEN) { Name = "pnp-release" };
        }
        public override SkillResult Run(Vector3 pickPosition, Vector3 pickOrientation, Vector3 placePosition, Vector3 placeOrientation)
        {
            _preGrasp.Execute(pickPosition + prePickOffset, pickOrientation);
            _openGripper.Execute();
            _grasp.Execute(pickPosition, pickOrientation);
            _closeGripper.Execute();
            _pickup.Execute(pickPosition + prePickOffset, pickOrientation);
            _prePlace.Execute(placePosition + prePlaceOffset, placeOrientation);
            _place.Execute(placePosition, placeOrientation);
            _release.Execute();
            return SkillResult.Success(new object[0], System.TimeSpan.Zero);
        }
        public override async Task<SkillResult> RunAsync(Vector3 pickPosition, Vector3 pickOrientation, Vector3 placePosition, Vector3 placeOrientation)
        {
            await _preGrasp.ExecuteAsync(pickPosition + prePickOffset, pickOrientation);
            await _openGripper.ExecuteAsync();
            await _grasp.ExecuteAsync(pickPosition, pickOrientation);
            await _closeGripper.ExecuteAsync();
            await _pickup.ExecuteAsync(pickPosition + prePickOffset, pickOrientation);
            await _prePlace.ExecuteAsync(placePosition + prePlaceOffset, placeOrientation);
            await _place.ExecuteAsync(placePosition, placeOrientation);
            // await _release.ExecuteAsync();
            return SkillResult.Success(new object[0], System.TimeSpan.Zero);
        }
    }
}