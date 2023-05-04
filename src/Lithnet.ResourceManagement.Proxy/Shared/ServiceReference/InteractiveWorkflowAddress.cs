using System;
using System.Collections.Generic;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable]
    public class InteractiveWorkflowAddress
    {
        public List<string> EndpointAddresses { get; set; }

        public Guid InstanceId { get; set; }
    }
}