using System;
using System.Collections.Generic;
using System.Text;

namespace Lithnet.ResourceManagement.Client.Hosts
{
    internal interface IPipeHost
    {
        string HostLocation { get; }

        void OpenPipe(string pipeName, ResourceManagementClientOptions p);
    }
}
