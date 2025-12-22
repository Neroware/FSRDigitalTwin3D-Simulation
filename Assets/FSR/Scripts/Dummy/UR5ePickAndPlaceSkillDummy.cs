using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.Robotics.Controller;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Dummy {

    public class UR5CobotPickAndPlaceSkillDummy : MonoBehaviour {

        [SerializeField] private GameObject pickTarget;
        [SerializeField] private GameObject placeLocation;
        [SerializeField] private Vector3 pickOffset;
        [SerializeField] private Vector3 placeOffset;
        [SerializeField] private PickAndPlaceBase pnp;
        [SerializeField] private RosMoveitPickAndPlaceController _pnpController;
        [SerializeField] private RosMoveitController _controller;
        public async void RunPickAndPlaceSkillTest()
        {            
            await pnp.RunAsync(pickTarget.transform.position + pickOffset, new Vector3(-180, 0, 0), 
                placeLocation.transform.position + placeOffset, new Vector3(-180, 0, 0));
        }
        public async void RunPickAndPlaceControllerTest()
        {
            await _pnpController.PlanAndRunAsync();
        }
        public async void RunMotionTest()
        {
            await _controller.PlanAndRunAsync();
        }
    }
}