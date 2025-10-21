// using System.Threading.Tasks;
// using FSR.DigitalTwin.Client.Features.UnityClient;
// using Unity.VisualScripting;

// namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Operator
// {

//     public class PickAndPlaceOperator : SkillBasedOperator
//     {
//         private bool _isBusy = false;
//         private string _runningOperation = "idle";

//         public override bool IsBusy => _isBusy;
//         public override string RunningOperation => _runningOperation;

//         protected override bool OnPull()
//         {
//             _isBusy = DigitalWorkspace.Instance.Entities.GetComponentProperty<bool>(Id.ToSafeString(), "is_busy");
//             return true;
//         }
//         protected override async Task<bool> OnPullAsync()
//         {
//             _isBusy = await DigitalWorkspace.Instance.Entities.GetComponentPropertyAsync<bool>(Id.ToSafeString(), "is_busy");
//             return true;
//         }
//         protected override bool OnPush()
//         {
//             return DigitalWorkspace.Instance.Entities.SetComponentProperty(Id.ToSafeString(), "is_busy", IsBusy);
//         }

//         protected override async Task<bool> OnPushAsync()
//         {
//             return await DigitalWorkspace.Instance.Entities.SetComponentPropertyAsync(Id.ToSafeString(), "is_busy", IsBusy);
//         }

//         protected override void OnInitComponent()
//         {
//             // TODO implement
//             base.OnInitComponent();
//         }
//     }

// }