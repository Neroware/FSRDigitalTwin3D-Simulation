using System;
using System.Threading.Tasks;
using UniRx;

namespace FSR.DigitalTwin.Client.Features.UnityClient.Interfaces {

    public interface IDigitalWorkspaceServerConnection : IDisposable {

        ReadOnlyReactiveProperty<bool> IsConnected { get; }
        IObservable<ClientNotificationBase> OnNotify { get; }

        Task<bool> Connect(string[] connArgs = null);
        Task<bool> Disconnect();

        Task Notify(ServerNotificationBase message);
    }

}