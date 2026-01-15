using System.Threading.Tasks;
using UniRx;

namespace FSR.DigitalTwin.Client.Features.Robotics.Interfaces
{
    public interface IScrewerController
    {
        ReadOnlyReactiveProperty<float> PercentComplete { get; }
        void PrepareScrew();
        Task PrepareScrewAsync();
        void ReleaseScrew();
        Task ReleaseScrewAsync();
        void ScrewIn();
        Task ScrewInAsync();
        void ScrewOut();
        Task ScrewOutAsync();
        void StopScrewer();
    }
}