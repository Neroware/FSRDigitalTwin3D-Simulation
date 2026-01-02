// using System;
// using System.Threading.Tasks;
// using UnityEngine;

// namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill
// {
//     public abstract class ScrewBase : OperatorSkillBase
//     {
//         [SerializeField] protected Vector3 eeOffset;
//         [SerializeField] protected Vector3 eeOrientation;
//         [SerializeField] protected bool startPositionEnabled = false;
//         [SerializeField] protected Vector3 startPosition = Vector3.zero;
//         protected enum EPrimitives
//         {
//             START, PRE_SCREW, SCREW, POST_SCREW, END
//         }
//         public void SetStartPosition(Vector3 startPosition)
//         {
//             startPositionEnabled = true;
//             this.startPosition = startPosition;
//         }
//         private bool GetMappedInput(in object[] input, out Transform target, out Vector3 orientation)
//         {
//             target = null;
//             orientation = Vector3.zero;
//             if (input.Length < 2 || input[0] is not Transform || input[1] is not Vector3)
//                 return false;
//             target = (Transform) input[0];
//             orientation = (Vector3) input[1];
//             return true;
//         }
//         public override SkillResult Run(string task, object[] inputs, object[] inOuts)
//         {
//             if (!GetMappedInput(inputs, out Transform target, out Vector3 orientation))
//             {
//                 return SkillResult.Failure(TimeSpan.Zero, "Bad input");
//             }
//             return Run(task, target, orientation);
//         }
//         public override async Task<SkillResult> RunAsync(string task, object[] inputs, object[] inOuts)
//         {
//             if (!GetMappedInput(inputs, out Transform target, out Vector3 orientation))
//             {
//                 return SkillResult.Failure(TimeSpan.Zero, "Bad input");
//             }
//             return await RunAsync(task, target, orientation);
//         }
//         public abstract SkillResult Run(string task, Transform target, Vector3 orientation);
//         public abstract Task<SkillResult> RunAsync(string task, Transform target, Vector3 orientation);
//     }
// }