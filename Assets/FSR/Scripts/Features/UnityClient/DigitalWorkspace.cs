using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;

namespace FSR.DigitalTwin.Client.Features.UnityClient {

    public static class DigitalWorkspace {

        public enum EOperationMode {
            Sleep = 0, Pull = 1, Push = 2
        };

        private static IDigitalWorkspace _workspace = null;
        public static IDigitalWorkspace Instance => _workspace;

        public static void SetWorkspace(IDigitalWorkspace ws) {
            _workspace ??= ws;
        }

    } 

}
