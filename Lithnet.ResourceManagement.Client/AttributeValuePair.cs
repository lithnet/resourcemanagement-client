using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public class AttributeValuePair
    {
        public string AttributeName { get; set; }

        public object Value { get; set; }

        public AttributeValuePair()
        {
        }

        public AttributeValuePair(string attributeName, object value)
        {
            this.AttributeName = attributeName;
            this.Value = value;
        }
    }
}
