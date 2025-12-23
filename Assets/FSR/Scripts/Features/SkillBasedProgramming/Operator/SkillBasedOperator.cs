// TODO Erbe aus SocialOperatorBase und passe OnFunction(...) an...
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Operator
{
    public class SkillBasedOperator : SocialOperatorBase
    {
        [SerializeField] private GameObject _skillList = null;
        private readonly Dictionary<string, OperatorSkillBase> _skills = new();
        private readonly Dictionary<string, OperatorSkillBase> _shortIds = new();
        private bool _isBusy = false;
        private string _runningOperation = null;

        public override bool IsBusy => _isBusy;
        public override string RunningOperation => _runningOperation;

        protected override void OnInitComponent()
        {
            FindSkills(_skillList ??= gameObject);
        }

        private void FindSkills(GameObject skillList)
        {
            var skills = skillList.GetComponents<OperatorSkillBase>();
            foreach (var skill in skills)
            {
                if (_skills.ContainsKey(skill.Id.ToString()))
                {
                    Debug.LogError($"Duplicate function {skill.Id} found in operator {OperatorId}");
                    continue;
                }
                _skills.Add(skill.Id.ToString(), skill);
                foreach(var shortId in skill.ShortIds)
                {
                    _shortIds[shortId] = skill;
                }
            }
        }

        protected override async Task<SkillResult> OnRun(string operation, object[] inputs, object[] inOuts)
        {
            if (_isBusy) throw new InvalidOperationException("Cannot launch operation on a busy operator");
            if (_shortIds.TryGetValue(operation, out OperatorSkillBase skill))
            {
                _isBusy = true;
                _runningOperation = operation;
                var res = await skill.RunAsync(inputs, inOuts);
                _isBusy = false;
                _runningOperation = "";
                return res;
            }
            else if (_skills.TryGetValue(operation, out OperatorSkillBase skill0))
            {
                _isBusy = true;
                _runningOperation = operation;
                var res = await skill0.RunAsync(inputs, inOuts);
                _isBusy = false;
                _runningOperation = "";
                return res;
            }
            Debug.LogError($"Unknown operation '{operation}' in operator '{OperatorId}'");
            return new SkillResult() { Succeeded = false, TimeExpired = TimeSpan.Zero };
        }

        public override bool CanRun(string shortId) => _shortIds.ContainsKey(shortId);
        public override bool CanRun(Uri skillUri) => _skills.ContainsKey(skillUri.ToString());
    }
}