using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Lithnet.ResourceManagement.Client
{
    public class AttributeValueCollection : IEnumerable<AttributeValue>
    {
        private Dictionary<string, AttributeValue> internalValues;

        internal AttributeValueCollection()
        {
            this.internalValues = new Dictionary<string, AttributeValue>();
        }

        public AttributeValue this[string attributeName]
        {
            get
            {
                if (this.internalValues.ContainsKey(attributeName))
                {
                    return this.internalValues[attributeName];
                }
                else
                {
                    throw new NoSuchAttributeException(attributeName);
                }
            }
            internal set
            {
                if (this.internalValues.ContainsKey(attributeName))
                {
                    this.internalValues[attributeName] = value;
                }
                else
                {
                    throw new NoSuchAttributeException(attributeName);
                }
            }
        }

        internal void Add(string attributeName, AttributeValue value)
        {
            this.internalValues.Add(attributeName, value);
        }

        internal void Add(AttributeValue value)
        {
            if (value.AttributeName == null)
            {
                throw new ArgumentNullException("value.AttributeName");
            }

            this.internalValues.Add(value.AttributeName, value);
        }

        public bool ContainsAttribute(string attributeName)
        {
            return this.internalValues.ContainsKey(attributeName);
        }

        public System.Collections.Generic.IEnumerator<AttributeValue> GetEnumerator()
        {
            return this.internalValues.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.internalValues.Values.GetEnumerator();
        }
    }
}
