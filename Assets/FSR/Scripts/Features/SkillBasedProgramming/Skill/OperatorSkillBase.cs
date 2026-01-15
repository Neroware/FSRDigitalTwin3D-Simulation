using System;
using System.Collections.Generic;
using System.Linq;
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
        protected abstract IEnumerable<IDevicePrimitive> GetPrimitivePlan(object[] inputs, object[] inOuts);
        protected virtual SkillResult OnRun(string task, List<IDevicePrimitive> primitives)
        {
            SkillResult result = new();
            for (int i = 0; i < primitives.Count; i++)
            {
                var res = primitives[i].Execute(task);
                if (res.Failed)
                {
                    // TODO Use clock to determine time delta
                    return SkillResult.Failure(TimeSpan.Zero,
                        $"Device primtive '{primitives[i].Name}' with Id {primitives[i].Id} failed");
                }
            }
            // TODO Use clock to determine time delta
            return result with { Succeeded = true, TimeExpired = TimeSpan.Zero };
        }
        protected virtual async Task<SkillResult> OnRunAsync(string task, List<IDevicePrimitive> primitives)
        {
            SkillResult result = new();
            for (int i = 0; i < primitives.Count; i++)
            {
                var res = await primitives[i].ExecuteAsync(task);
                if (res.Failed)
                {
                    // TODO Use clock to determine time delta
                    return SkillResult.Failure(TimeSpan.Zero,
                        $"Device primtive '{primitives[i].Name}' with Id {primitives[i].Id} failed");
                }
            }
            // TODO Use clock to determine time delta
            return result with { Succeeded = true, TimeExpired = TimeSpan.Zero };
        }
        public SkillResult Run(string task, object[] inputs, object[] inOuts)
        {
            List<IDevicePrimitive> primitives = GetPrimitivePlan(inputs, inOuts).ToList();
            return OnRun(task, primitives);
        }
        public async Task<SkillResult> RunAsync(string task, object[] inputs, object[] inOuts)
        {
            List<IDevicePrimitive> primitives = GetPrimitivePlan(inputs, inOuts).ToList();
            return await OnRunAsync(task, primitives);
        }
    }
}