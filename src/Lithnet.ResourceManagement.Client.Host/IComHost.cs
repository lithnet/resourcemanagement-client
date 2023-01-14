using System.Runtime.InteropServices;

namespace Lithnet.ResourceManagement.Client.Host
{
    [ComVisible(true)]
    [Guid("47fce021-1c5c-419c-95a9-b0a379efaad2")]
    public interface IComHost
    {
        void ClosePipe(string pipeName);

        void OpenPipe(string pipeName);
    }
}