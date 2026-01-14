using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.Assembly.ScrewBehavior;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.Player
{
    public class PickAndPlace : PickAndPlaceBase
    {
        // TODO Use generalized 'interactivity' primitive
        private class AwaitInteractPrimitive : IDevicePrimitive
        {
            public string Name => "await-interact";
            public Uri Id => UriPrefix.PI + Name;
            public Hole Hole { init; get; }

            public PrimitiveResult Execute(string task)
            {
                return ExecuteAsync(task).Result;
            }
            public Task<PrimitiveResult> ExecuteAsync(string task)
            {
                PrimitiveResult res = new() { Outputs = new object[0] };
                if (Hole == null || Hole.ScrewPart != null)
                    return Task.FromResult(res);
                return Hole.ScrewPartPlaced
                    .Where(sp => sp != null)
                    .First()
                    .Select(_ => res)
                    .ToTask();
            }
        }

        protected override IEnumerable<IDevicePrimitive> GetPrimitivePlan(Transform pickTarget, Vector3 pickOrientation, Transform placeTarget, Vector3 placeOrientation)
        {
            yield return new AwaitInteractPrimitive()
            {
                Hole = placeTarget.GetComponent<Hole>()
            };
        }
    }
}