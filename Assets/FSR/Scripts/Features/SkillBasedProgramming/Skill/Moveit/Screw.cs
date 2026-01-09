using System;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive.Native;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.Moveit
{
    public class Screw : ScrewBase
    {
        [SerializeField] private RosMoveitController _controller;
        [SerializeField] private NativeScrewEEController _eeController;
        [SerializeField] private Vector3 screwOffset = Vector3.zero;
        // Parameters
        public Vector3 ScrewOffset { set => screwOffset = value; get => screwOffset; }

        protected override IEnumerable<IDevicePrimitive> GetPrimitivePlan(Transform target, Vector3 orientation)
        {
            yield return new IfThen(_ => startPositionEnabled)
            {
                Then = new Primitive.Moveit.Motion(_controller)
                {
                    Name = "screw-start",
                    Target = StartPosition - EEOffset,
                    Orientation = orientation - EEOrientation
                }
            };
            yield return new Primitive.Moveit.Motion(_controller)
            {
                Name = "screw-pre-screw",
                Target = target.position - EEOffset + ScrewOffset,
                Orientation = orientation - EEOrientation
            };
            yield return new Composite(new List<IDevicePrimitive>()
            {
                new ScrewerTool(_eeController)
                {
                    ScrewPathLength = 0.025f,
                    ScrewSpeed = 0.01f
                },
                new Primitive.Moveit.Motion(_controller)
                {
                    Name = "screw-screw",
                    Target = target.position - EEOffset,
                    Orientation = orientation - EEOrientation,
                    SpeedScale = 0.05f
                }
            });
            yield return new Primitive.Moveit.Motion(_controller)
            {
                Name = "screw-post-screw",
                Target = target.position - EEOffset + ScrewOffset,
                Orientation = orientation - EEOrientation
            };
            yield return new IfThen(_ => startPositionEnabled)
            {
                Then = new Primitive.Moveit.Motion(_controller)
                {
                    Name = "screw-end",
                    Target = StartPosition - EEOffset,
                    Orientation = orientation - EEOrientation
                }
            };
        }
    }
}