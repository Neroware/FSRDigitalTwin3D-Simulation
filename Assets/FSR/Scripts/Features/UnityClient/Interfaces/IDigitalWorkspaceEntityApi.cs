using System.Threading.Tasks;

namespace FSR.DigitalTwin.Client.Features.UnityClient.Interfaces {
    public interface IDigitalWorkspaceEntityApi {
        bool HasEntity(string id);
        Task<bool> HasEntityAsync(string id);

        bool HasComponent(string assetId, string componentId);
        Task<bool> HasComponentAsync(string assetId, string componentId);

        string[] GetEntities();
        Task<string[]> GetEntitesAsync();

        string[] GetEntityComponents(string assetId);
        Task<string[]> GetEntityComponentsAsync(string assetId);

        bool CreateComponentProperty<T>(string id, string prop, T value);
        Task<bool> CreateComponentPropertyAsync<T>(string id, string prop, T value);
        bool HasComponentProperty(string id, string prop);
        Task<bool> HasComponentPropertyAsync(string id, string prop);
        bool SetComponentProperty<T>(string id, string prop, T value);
        Task<bool> SetComponentPropertyAsync<T>(string id, string prop, T value);
        T GetComponentProperty<T>(string id, string prop);
        Task<T> GetComponentPropertyAsync<T>(string id, string prop);
    }
}