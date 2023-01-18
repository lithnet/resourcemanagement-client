using System;
using System.Collections.Generic;
using System.Text;

namespace Lithnet.ResourceManagement.Client.Hosts
{
    internal interface IPipeHost
    {
        void OpenPipe(string pipeName);
    }
}
