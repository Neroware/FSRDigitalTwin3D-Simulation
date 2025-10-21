using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3.Services.AssetAdministrationShellRepository;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3.Services.AssetAdministrationShellService;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3.Services.Services.SubmodelService;
using FSR.DigitalTwin.Client.Features.UnityClient.GRPC.AAS.Utils;
using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;
using Grpc.Core;

namespace FSR.DigitalTwin.Client.Features.UnityClient.GRPC.AAS {

    public class GrpcDigitalWorkspaceApiBridge : IDigitalWorkspaceEntityApi
    {
        private readonly GrpcAdminShellApiServiceClient _client;
        private static OutputModifier DefaultOutput = new() { 
            Content = OutputContent.Normal, 
            Extent = OutputExtent.WithoutBlobValue, 
            Level = OutputLevel.Deep,
            Cursor = "",
            Limit = 32
        };

        public GrpcDigitalWorkspaceApiBridge(Channel channel) {
            _client = new(channel);
        }

        public async Task<string[]> GetEntitesAsync()
        {
            GetAllAssetAdministrationShellsRpcRequest request = new() { OutputModifier = DefaultOutput };
            var response = await _client.AdminShellRepo.GetAllAssetAdministrationShellsAsync(request);
            return response.Payload.Select(x => x.Id).ToArray();
        }

        public string[] GetEntities()
        {
            GetAllAssetAdministrationShellsRpcRequest request = new() { OutputModifier = DefaultOutput };
            var response = _client.AdminShellRepo.GetAllAssetAdministrationShells(request);
            return response.Payload.Select(x => x.Id).ToArray();
        }

        public async Task<string[]> GetEntityComponentsAsync(string assetId)
        {
            GetAllSubmodelReferencesRpcRequest refRequest = new() { Id = Base64Converter.ToBase64(assetId), OutputModifier = DefaultOutput };
            var refResponse = await _client.AdminShell.GetAllSubmodelReferencesAsync(refRequest);
            return refResponse.Payload.Select(r => r.Keys.First().Value).ToArray();
        }

        public string[] GetEntityComponents(string assetId)
        {
            GetAllSubmodelReferencesRpcRequest refRequest = new() { Id = Base64Converter.ToBase64(assetId), OutputModifier = DefaultOutput };
            var refResponse = _client.AdminShell.GetAllSubmodelReferences(refRequest);
            return refResponse.Payload.Select(r => r.Keys.First().Value).ToArray();
        }

        public bool HasComponent(string assetId, string componentId)
        {
            return GetEntityComponents(assetId).Contains(componentId);
        }

        public async Task<bool> HasComponentAsync(string assetId, string componentId)
        {
            var components = await GetEntityComponentsAsync(assetId);
            return components.Contains(componentId);
        }

        public bool HasEntity(string id)
        {
            GetAssetAdministrationShellByIdRpcRequest request = new() { Id = Base64Converter.ToBase64(id), OutputModifier = DefaultOutput };
            var response = _client.AdminShellRepo.GetAssetAdministrationShellById(request);
            return response.StatusCode == 200;
        }

        public async Task<bool> HasEntityAsync(string id)
        {
            GetAssetAdministrationShellByIdRpcRequest request = new() { Id = Base64Converter.ToBase64(id), OutputModifier = DefaultOutput };
            var response = await _client.AdminShellRepo.GetAssetAdministrationShellByIdAsync(request);
            return response.StatusCode == 200;
        }

        public bool SetComponentProperty<T>(string id, string prop, T value)
        {
            SubmodelElementDTO property = CreateProperty(value);
            string[] path = prop.Split('.');
            property.IdShort = path.Last();
            PutSubmodelElementByPathRpcRequest request = new() { SubmodelId = Base64Converter.ToBase64(id), SubmodelElement = property };
            foreach (string idShort in path) {
                request.Path.Add(new KeyDTO() { Type = KeyTypes.SubmodelElement, Value = idShort });
            }
            var response = _client.Submodel.PutSubmodelElementByPath(request);
            return response.StatusCode == 201;
        }

        public async Task<bool> SetComponentPropertyAsync<T>(string id, string prop, T value)
        {
            SubmodelElementDTO property = CreateProperty(value);
            string[] path = prop.Split('.');
            property.IdShort = path.Last();
            PutSubmodelElementByPathRpcRequest request = new() { SubmodelId = Base64Converter.ToBase64(id), SubmodelElement = property };
            foreach (string idShort in path) {
                request.Path.Add(new KeyDTO() { Type = KeyTypes.SubmodelElement, Value = idShort });
            }
            var response = await _client.Submodel.PutSubmodelElementByPathAsync(request);
            return response.StatusCode == 201;
        }

        private SubmodelElementDTO CreateProperty<T>(T value) {
            var propValue = DataTypeDefXsdConverter.Convert(value);
            return SubmodelElementFactory.Create(SubmodelElementType.Property, null, propValue.Item2, propValue.Item1);
        }

        public bool CreateComponentProperty<T>(string id, string prop, T value)
        {
            string[] path = prop.Split('.');
            if (path.Length == 0) {
                return false;
            }
            SubmodelElementDTO sme = CreateProperty(value);

            int i = 0;
            for (; i < path.Length - 1; i++) {
                SubmodelElementDTO pathElem = GetSubmodelElementByPath(id, path[..(i + 1)]);
                if (pathElem == null) {
                    break;
                }
                if (pathElem.SubmodelElementType != SubmodelElementType.SubmodelElementList 
                    && pathElem.SubmodelElementType != SubmodelElementType.SubmodelElementCollection) {
                        return false;
                }
            }

            string[] prefix = path[..i];
            string[] postfix = path[i..];
            for (int j = 0; j < postfix.Length; j++) {
                sme.IdShort = postfix[postfix.Length - 1 - j];
                if (j == postfix.Length - 1) {
                    break;
                }
                sme = SubmodelElementFactory.Create(SubmodelElementType.SubmodelElementCollection, new List<SubmodelElementDTO>(){ sme });
            }

            PostSubmodelElementByPathRpcRequest request = new() { SubmodelId = Base64Converter.ToBase64(id), SubmodelElement = sme };
            foreach (string idShort in prefix) {
                request.Path.Add(new KeyDTO() { Type = KeyTypes.SubmodelElement, Value = idShort });
            }
            var response = _client.Submodel.PostSubmodelElementByPath(request);
            return response.StatusCode == 201;
        }

        public async Task<bool> CreateComponentPropertyAsync<T>(string id, string prop, T value)
        {
            string[] path = prop.Split('.');
            if (path.Length == 0) {
                return false;
            }
            SubmodelElementDTO sme = CreateProperty(value);

            int i = 0;
            for (; i < path.Length - 1; i++) {
                SubmodelElementDTO pathElem = await GetSubmodelElementByPathAsync(id, path[..(i + 1)]);
                if (pathElem == null) {
                    break;
                }
                if (pathElem.SubmodelElementType != SubmodelElementType.SubmodelElementList 
                    && pathElem.SubmodelElementType != SubmodelElementType.SubmodelElementCollection) {
                        return false;
                }
            }

            string[] prefix = path[..i];
            string[] postfix = path[i..];
            for (int j = 0; j < postfix.Length; j++) {
                sme.IdShort = postfix[postfix.Length - 1 - j];
                if (j == postfix.Length - 1) {
                    break;
                }
                sme = SubmodelElementFactory.Create(SubmodelElementType.SubmodelElementCollection, new List<SubmodelElementDTO>(){ sme });
            }

            PostSubmodelElementByPathRpcRequest request = new() { SubmodelId = Base64Converter.ToBase64(id), SubmodelElement = sme };
            foreach (string idShort in prefix) {
                request.Path.Add(new KeyDTO() { Type = KeyTypes.SubmodelElement, Value = idShort });
            }
            var response = await _client.Submodel.PostSubmodelElementByPathAsync(request);
            return response.StatusCode == 201;
        }

        private SubmodelElementDTO GetSubmodelElementByPath(string id, string[] idShortPath) {
            if (idShortPath.Length == 1) {
                GetAllSubmodelElementsRpcRequest allRequest = new() { SubmodelId = Base64Converter.ToBase64(id), OutputModifier = DefaultOutput };
                var allResponse = _client.Submodel.GetAllSubmodelElements(allRequest);
                if (allResponse.StatusCode != 200) {
                    return null;
                }
                var result = allResponse.Payload.Where(sme => sme.IdShort == idShortPath.Last());
                return result.Count() >= 1 ? result.First() : null;
            }
            GetSubmodelElementByPathRpcRequest request = new() { SubmodelId = Base64Converter.ToBase64(id), OutputModifier = DefaultOutput };
            foreach (string idShort in idShortPath) {
                request.Path.Add(new KeyDTO() { Type = KeyTypes.SubmodelElement, Value = idShort });
            }
            var response = _client.Submodel.GetSubmodelElementByPath(request);
            return response.StatusCode == 200 ? response.Payload : null;
        }

        private async Task<SubmodelElementDTO> GetSubmodelElementByPathAsync(string id, string[] idShortPath) {
            if (idShortPath.Length == 1) {
                GetAllSubmodelElementsRpcRequest allRequest = new() { SubmodelId = Base64Converter.ToBase64(id), OutputModifier = DefaultOutput };
                var allResponse = await _client.Submodel.GetAllSubmodelElementsAsync(allRequest);
                if (allResponse.StatusCode != 200) {
                    return null;
                }
                var result = allResponse.Payload.Where(sme => sme.IdShort == idShortPath.Last());
                return result.Count() >= 1 ? result.First() : null;
            }
            GetSubmodelElementByPathRpcRequest request = new() { SubmodelId = Base64Converter.ToBase64(id), OutputModifier = DefaultOutput };
            foreach (string idShort in idShortPath) {
                request.Path.Add(new KeyDTO() { Type = KeyTypes.SubmodelElement, Value = idShort });
            }
            var response = await _client.Submodel.GetSubmodelElementByPathAsync(request);
            return response.StatusCode == 200 ? response.Payload : null;
        }

        public bool HasComponentProperty(string id, string prop)
        {
            string[] idShortPath = prop.Split('.');
            var result = GetSubmodelElementByPath(id, idShortPath);
            return result != null && result.SubmodelElementType == SubmodelElementType.Property;
        }

        public async Task<bool> HasComponentPropertyAsync(string id, string prop)
        {
            string[] idShortPath = prop.Split('.');
            var result = await GetSubmodelElementByPathAsync(id, idShortPath);
            return result != null && result.SubmodelElementType == SubmodelElementType.Property;
        }

        private T GetProperty<T>(PropertyPayloadDTO property) {
            return DataTypeDefXsdConverter.Convert<T>(property.ValueType, property.Value);
        }

        public T GetComponentProperty<T>(string id, string prop)
        {
            GetSubmodelElementByPathRpcRequest request = new() { SubmodelId = Base64Converter.ToBase64(id), OutputModifier = DefaultOutput };
            foreach (string idShort in prop.Split('.')) {
                request.Path.Add(new KeyDTO() { Type = KeyTypes.SubmodelElement, Value = idShort });
            }
            var response = _client.Submodel.GetSubmodelElementByPath(request);
            if (response.StatusCode != 200 || response.Payload.SubmodelElementType != SubmodelElementType.Property) {
                throw new NullReferenceException("Property not found!");
            }
            return GetProperty<T>(response.Payload.Property);
        }

        public async Task<T> GetComponentPropertyAsync<T>(string id, string prop)
        {
            GetSubmodelElementByPathRpcRequest request = new() { SubmodelId = Base64Converter.ToBase64(id), OutputModifier = DefaultOutput };
            foreach (string idShort in prop.Split('.')) {
                request.Path.Add(new KeyDTO() { Type = KeyTypes.SubmodelElement, Value = idShort });
            }
            var response = await _client.Submodel.GetSubmodelElementByPathAsync(request);
            if (response.StatusCode != 200 || response.Payload.SubmodelElementType != SubmodelElementType.Property) {
                throw new NullReferenceException("Property not found!");
            }
            return GetProperty<T>(response.Payload.Property);
        }
    }

}