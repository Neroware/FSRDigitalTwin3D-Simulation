using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill
{
    public abstract class ScrewBase : OperatorSkillBase
    {
        [SerializeField] private Vector3 eeOffset;
        [SerializeField] private Vector3 eeOrientation;
        [SerializeField] protected bool startPositionEnabled = false;
        [SerializeField] private Vector3 startPosition = Vector3.zero;

        // Parameters
        public Vector3 EEOffset { set => eeOffset = value; get => eeOffset; }
        public Vector3 EEOrientation { set => eeOrientation = value; get => eeOrientation; }
        public Vector3 StartPosition { set => SetStartPosition(value); get => startPosition; }
        public void SetStartPosition(Vector3 startPosition)
        {
            startPositionEnabled = true;
            this.startPosition = startPosition;
        }
        public enum EPrimitives
        {
            START, PRE_SCREW, SCREW, POST_SCREW, END
        }
        private bool GetMappedInput(in object[] input, out Transform target, out Vector3 orientation)
        {
            target = null;
            orientation = Vector3.zero;
            if (input.Length < 2 || input[0] is not Transform || input[1] is not Vector3)
                return false;
            target = (Transform) input[0];
            orientation = (Vector3) input[1];
            return true;
        }
        protected override IEnumerable<IDevicePrimitive> GetPrimitivePlan(object[] inputs, object[] inOuts)
        {
            if (!GetMappedInput(inputs, out Transform target, out Vector3 orientation))
            {
                Debug.LogError("Bad input! Failed to construct skill primitive plan!");
                return new IDevicePrimitive[0];
            }
            return GetPrimitivePlan(target, orientation);
        }
        protected abstract IEnumerable<IDevicePrimitive> GetPrimitivePlan(Transform target, Vector3 orientation);
        public SkillResult Run(string task, Transform target, Vector3 orientation)
        {
            List<IDevicePrimitive> primitives = GetPrimitivePlan(target, orientation).ToList();
            return OnRun(task, primitives);
        }
        public Task<SkillResult> RunAsync(string task, Transform target, Vector3 orientation)
        {
            List<IDevicePrimitive> primitives = GetPrimitivePlan(target, orientation).ToList();
            return OnRunAsync(task, primitives);
        }
    }
}