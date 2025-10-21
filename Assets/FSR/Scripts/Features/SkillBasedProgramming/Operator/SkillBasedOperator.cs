// TODO Erbe aus SocialOperatorBase und passe OnFunction(...) an...
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            var functions = skillList.GetComponents<OperatorSkillBase>();
            foreach (var function in functions)
            {
                if (_skills.ContainsKey(function.Id.ToString()))
                {
                    Debug.LogError($"Duplicate function {function.Id} found in operator {OperatorId}");
                    continue;
                }
                _skills.Add(function.Id.ToString(), function);
                foreach(var shortId in function.ShortIds)
                {
                    _shortIds[shortId] = function;
                }
            }
        }

        protected override async Task<SkillResult> OnFunction(string function, object[] inputs, object[] inOuts)
        {
            if (_isBusy) throw new InvalidOperationException("Cannot launch function on a busy operator");
            if (_shortIds.TryGetValue(function, out OperatorSkillBase skill))
            {
                _isBusy = true;
                _runningOperation = function;
                var res = await skill.RunAsync(inputs, inOuts);
                _isBusy = false;
                _runningOperation = "";
                return res;
            }
            else if (_skills.TryGetValue(function, out OperatorSkillBase skill0))
            {
                _isBusy = true;
                _runningOperation = function;
                var res = await skill0.RunAsync(inputs, inOuts);
                _isBusy = false;
                _runningOperation = "";
                return res;
            }
            Debug.LogError($"Unknown function '{function}' in operator '{OperatorId}'");
            return new SkillResult() { Succeeded = false, TimeExpired = TimeSpan.Zero };
        }

        public override bool CanRun(string shortId) => _shortIds.ContainsKey(shortId);
        public override bool CanRun(Uri skillUri) => _skills.ContainsKey(skillUri.ToString());
    }
}