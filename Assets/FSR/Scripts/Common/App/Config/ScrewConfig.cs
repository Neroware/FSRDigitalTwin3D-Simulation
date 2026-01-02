using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill.Moveit;
using UnityEngine;

namespace FSR.DigitalTwin.Client.App.Config
{
    public class ScrewConfig : MonoBehaviour
    {
        public Screw skill; 
        public GameObject target;
        public Vector3 orientation;
        public Vector3 screwOffset;
        public Vector3 eeOrientation;
        public Vector3 eeOffset;
    }
}