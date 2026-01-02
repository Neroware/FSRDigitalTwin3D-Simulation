// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using FSR.DigitalTwin.Client.Features.Robotics.Controller;
// using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
// using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive;
// using UnityEngine;

// namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.UR5e
// {
//     public class Screw : ScrewBase
//     {
//         [SerializeField] private RosMoveitController _controller;
//         private Primitive.Moveit.Motion _start, _preScrew, _screw, _postScrew, _end;

//         public override List<IDevicePrimitive> Primitives => new() {
//             new IfThen(_ => startPositionEnabled, _start),
//             _preScrew,
//             _screw,
//             _postScrew,
//             new IfThen(_ => startPositionEnabled, _end),
//         };

//         private void Start()
//         {
            
//         }

//         public override SkillResult Run(string task, Transform target, Vector3 orientation)
//         {
            
//         }

//         public override Task<SkillResult> RunAsync(string task, Transform target, Vector3 orientation)
//         {
//             throw new NotImplementedException();
//         }
//     }
// }