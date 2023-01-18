using System;
using System.Runtime.InteropServices;

namespace Lithnet.ResourceManagement.Client.Host
{
    [ComVisible(true)]
    [ProgId("Lithnet.ResourceManagement.Client.ComHost")]
    [Guid("dbdea27a-3d96-483c-97d1-9047c70141ac")]
    public class ComHost : IComHost
    {
        private readonly PipeHost pipeHost = new PipeHost();

        public void OpenPipe(string pipeName)
        {
            try
            {
                this.pipeHost.OpenPipe(pipeName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "COM host is terminating due to an unhandled error");
                throw;
            }
        }

        public void ClosePipe(string pipeName)
        {
            this.pipeHost.ClosePipe(pipeName);
        }
    }
}
