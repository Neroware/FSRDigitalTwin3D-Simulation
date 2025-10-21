using FSR.DigitalTwin.Client.Common.Interfaces;

namespace FSR.DigitalTwin.Client.Common
{
    public static class VirtualWorkspace {
        private static IVirtualWorkspace _workspace = null;
        public static IVirtualWorkspace Instance => _workspace;

        public static void SetWorkspace(IVirtualWorkspace ws) {
            _workspace ??= ws;
        }

    } 
}