
namespace FSR.DigitalTwin.Client.Features.UnityClient.Interfaces {

    /// <summary>
    /// The digitized workspace maintained by the digital twin.
    /// 
    /// This interface shall allow access to the following aspects:<br />
    /// - Connection layer services<br />
    /// - Operational and process signals and callbacks<br />
    /// - An API for entities and their components<br />
    /// - A semantic knowledge base
    /// </summary>
    public interface IDigitalWorkspace
    {
        /// <summary>
        /// Maintains the connection to the simulation server (i.e. the digital twin)
        /// </summary>
        IDigitalWorkspaceServerConnection Connection { get; }
        /// <summary>
        /// Allows high-level initiation of processes
        /// </summary>
        IDigitalWorkspaceOperational Operational { get; }
        /// <summary>
        /// Entity and component API
        /// </summary>
        IDigitalWorkspaceEntityApi Entities { get; }
        /// <summary>
        /// Semantic knowledge base
        /// </summary>
        IDigitalWorkspaceKnowledge Knowledge { get; }

        DigitalWorkspace.EOperationMode OperationMode { set; get; }
        string WorkspaceName { get; }
    }

}
