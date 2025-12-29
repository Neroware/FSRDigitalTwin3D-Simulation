using System;
using System.Linq;
using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.DES.Utils
{
    /// <summary>
    /// A class matching URIs to supported HRCFunctions. Note: A function is not a skill. A skill is the implementation of
    /// an agent's capability, while a function represents the task, that needs to be carried out to act out the skill.
    /// 
    /// This is the reason, why this class is located in the DES feature and not under SkillBasedProgramming.
    /// </summary>
    [CreateAssetMenu(fileName = "UnnamedHRCFunctionFactory", menuName = "ScriptableObjects/DES/HRCFunctionFactory", order = 1)]
    public class HRCFunctionFactory : ScriptableObject, IHRCFunctionFactory
    {
        [SerializeField]
        private string[] pickPlaceUris = new string[]
        {
            UriPrefix.SOHO | "PickPlace"
        };

        [SerializeField]
        private string[] joinUris = new string[]
        {
            UriPrefix.SOHO | "Join",
            UriPrefix.SOHO | "Screw"
        };

        public HRCFunction Create(string taskId, HRCFunctionDescription description)
        {
            if (pickPlaceUris.Contains(description.FunctionType)) return CreatePickPlace(taskId, description);
            if (joinUris.Contains(description.FunctionType)) return CreateJoin(taskId, description);
            return new HRCFunction()
            {
                TaskId = taskId,
                TaskDescription = description,
                Inputs = new object[0],
                InOuts = new object[0]
            };
        }
        
        private HRCFunction CreateJoin(string taskId, HRCFunctionDescription description)
        {
            Transform screw = GameObject.Find(description.Goal)?.transform;
            Vector3 screwDirection = Vector3.down;
            object[] inputs = new object[] { screw, screwDirection };
            object[] inOuts = new object[0];
            return new HRCFunction()
            {
                TaskId = taskId,
                TaskDescription = description,
                Inputs = inputs,
                InOuts = inOuts
            };
        }
        
        private HRCFunction CreatePickPlace(string taskId, HRCFunctionDescription description)
        {
            Transform pick = null; // TODO Find location based on URI
            Vector3 pickDirection = Vector3.down;
            Transform place = GameObject.Find(description.Goal)?.transform;
            Vector3 placeDirection = Vector3.down;
            object[] inputs = new object[] { pick, pickDirection, place, placeDirection };
            object[] inOuts = new object[0];
            return  new HRCFunction() {
                TaskId = taskId,
                TaskDescription = description,
                Inputs = inputs,
                InOuts = inOuts
            };
        }
    }
}