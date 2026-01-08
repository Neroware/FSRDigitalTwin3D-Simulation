using System;
using System.Threading.Tasks;

namespace FSR.DigitalTwin.Client.Features.Robotics.Interfaces
{
    public interface IScrewerController
    {
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