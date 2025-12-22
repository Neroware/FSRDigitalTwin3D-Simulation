using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Skill;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Dummy {

    public class UR5CobotPickAndPlaceSkillDummy : MonoBehaviour {

        [SerializeField] private GameObject pickTarget;
        [SerializeField] private GameObject placeLocation;
        [SerializeField] private Vector3 pickOffset;
        [SerializeField] private Vector3 placeOffset;
        [SerializeField] private PickAndPlaceBase pnp;

        public async void RunPickAndPlaceSkillTest()
        {
            await pnp.RunAsync(pickTarget.transform.position + pickOffset, new Vector3(-180, 0, 0), 
                placeLocation.transform.position + placeOffset, new Vector3(-180, 0, 0));
        }

    }

}