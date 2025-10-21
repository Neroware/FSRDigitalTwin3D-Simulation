using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3.Services.AssetAdministrationShellRepository;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3.Services.AssetAdministrationShellService;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3.Services.Services.SubmodelService;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3.Services.SubmodelRepository;
using Grpc.Core;

namespace FSR.DigitalTwin.Client.Features.UnityClient.GRPC.AAS {

/// <summary>
/// API service client according to 'Details of the Asset Administration Shell - Part 2'
/// </summary>
public class GrpcAdminShellApiServiceClient {
    
    public AssetAdministrationShellService.AssetAdministrationShellServiceClient AdminShell { get; }
    public AssetAdministrationShellRepositoryService.AssetAdministrationShellRepositoryServiceClient AdminShellRepo { get; }
    public SubmodelService.SubmodelServiceClient Submodel { get; }
    public SubmodelRepositoryService.SubmodelRepositoryServiceClient SubmodelRepo { get; }

    public GrpcAdminShellApiServiceClient (Channel channel) {
        AdminShell = new(channel);
        AdminShellRepo = new(channel);
        Submodel = new(channel);
        SubmodelRepo = new(channel);
    }
}

} // END namespace FSR.Workspace.Digital.Services