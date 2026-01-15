using FSR.DigitalTwin.Client.Common.Utils;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.Moveit;
using UnityEngine;

namespace FSR.DigitalTwin.Client.App.Config
{
    public class PickAndPlaceConfig : MonoBehaviour
    {
        public PickAndPlace pnp; 
        public GameObject pickTarget;
        public GameObject placeLocation;
        public Vector3 pickOffset;
        public Vector3 pickOrientation;
        public Vector3 placeOffset;
        public Vector3 placeOrientation;
        public string taskId = "PnPTest";
    }
}