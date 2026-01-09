using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Primitive.Native
{
    public class ScrewerTool : ScrewerToolBase
    {
        private readonly NativeScrewEEController _controller;
        public ScrewerTool(NativeScrewEEController controller)
        {
            _controller = controller;
        }
        public override PrimitiveResult Execute(string task)
        {
            _controller.SetMode(ScrewSpeed < 0.0 ? NativeScrewEEController.EMode.OUT : NativeScrewEEController.EMode.IN);
            _controller.ScrewerPathLength = ScrewPathLength;
            _controller.ScrewerSpeed = Math.Abs(ScrewSpeed);
            _controller.PrepareScrew();
            _controller.Plan();
            if (_controller.ValidatePlan())
                _controller.RunPlan();
            _controller.ReleaseScrew();
            return PrimitiveResult.Success(new object[0]);
        }
        public override async Task<PrimitiveResult> ExecuteAsync(string task)
        {
            _controller.SetMode(ScrewSpeed < 0.0 ? NativeScrewEEController.EMode.OUT : NativeScrewEEController.EMode.IN);
            _controller.ScrewerPathLength = ScrewPathLength;
            _controller.ScrewerSpeed = Math.Abs(ScrewSpeed);
            await _controller.PrepareScrewAsync();
            await _controller.PlanAsync();
            if (_controller.ValidatePlan())
                await _controller.RunPlanAsync();
            await _controller.ReleaseScrewAsync();
            return PrimitiveResult.Success(new object[0]);
        }
    }
}