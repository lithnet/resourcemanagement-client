using System.Runtime.InteropServices;

namespace Lithnet.ResourceManagement.Client
{
    [ComImport]
    [Guid("47fce021-1c5c-419c-95a9-b0a379efaad2")]
    internal interface IComHost
    {
        void ClosePipe(string pipeName);

        void OpenPipe(string pipeName);
    }
}