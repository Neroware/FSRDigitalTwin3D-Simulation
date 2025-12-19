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
        public abstract object[] MapPrimitiveInput(int primitive, object[] inputs, object[] inOuts);
        public abstract void Execute(int primitive, object[] input, in SkillResult result);
        public abstract Task ExecuteAsync(int primitive, object[] input, SkillResult result);

        public SkillResult Run(object[] inputs, object[] inOuts)
        {
            SkillResult result = new();
            for (int i = 0; i < Primitives.Count; i++)
            {
                Execute(i, MapPrimitiveInput(i, inputs, inOuts), result);
            }
            return result;
        }
        public async Task<SkillResult> RunAsync(object[] inputs, object[] inOuts)
        {
            SkillResult result = new();
            for (int i = 0; i < Primitives.Count; i++)
            {
                await ExecuteAsync(i, MapPrimitiveInput(i, inputs, inOuts), result);
            }
            return result;
        }
    }
}