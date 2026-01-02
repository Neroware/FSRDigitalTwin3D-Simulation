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
            pnp.PickOffset = pickOffset;
            pnp.PlaceOffset = placeOffset;
            await pnp.RunAsync("pi:pnp-test", pickTarget.transform, new Vector3(-180, 0, 0), 
                placeLocation.transform, new Vector3(-180, 0, 0));
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