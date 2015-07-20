using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;

namespace Lithnet.ResourceManagement.Client
{
    public class AttributeValueChange
    {
        public AttributeValueChange(ModeType changeType, object value)
        {
            this.ChangeType = changeType;
            this.Value = value;
        }

        public ModeType ChangeType { get; internal set; }

        public object Value { get; internal set; }
    }
}
