using System;
using System.Collections.Generic;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    internal interface IPipeHost
    {
        string HostLocation { get; }

        void OpenPipe(string pipeName, ResourceManagementClientOptions p);
    }
}
