using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill
{
    public abstract class OperatorSkillBase : MonoBehaviour, IOperatorSkill
    {
        [SerializeField] private string _id;
        [SerializeField] private string[] _shortIds = null;
        public string[] ShortIds => _shortIds ?? new string[0];
        public Uri Id => new(_id);

        public abstract List<IDevicePrimitive> Primitives { get; }
        public virtual SkillResult Run(string task, object[] inputs, object[] inOuts)
        {
            SkillResult result = new();
            for (int i = 0; i < Primitives.Count; i++)
            {
                var res = Primitives[i].Execute(task, inputs);
                if (res.Failed)
                {
                    // TODO Use clock to determine time delta
                    return SkillResult.Failure(TimeSpan.Zero,
                        $"Device primtive '{Primitives[i].Name}' with Id {Primitives[i].Id} failed");
                }
            }
            // TODO Use clock to determine time delta
            return result with { Succeeded = true, TimeExpired = TimeSpan.Zero };
        }
        public virtual async Task<SkillResult> RunAsync(string task, object[] inputs, object[] inOuts)
        {
            SkillResult result = new();
            for (int i = 0; i < Primitives.Count; i++)
            {
                var res = await Primitives[i].ExecuteAsync(task, inputs);
                if (res.Failed)
                {
                    // TODO Use clock to determine time delta
                    return SkillResult.Failure(TimeSpan.Zero,
                        $"Device primtive '{Primitives[i].Name}' with Id {Primitives[i].Id} failed");
                }
            }
            // TODO Use clock to determine time delta
            return result with { Succeeded = true, TimeExpired = TimeSpan.Zero };
        }
    }
}