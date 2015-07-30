using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public class AttributeValuePairCollection: IEnumerable<AttributeValuePair>
    {
        private List<AttributeValuePair> internalList = new List<AttributeValuePair>();

        public AttributeValuePairCollection()
        {
        }

        public AttributeValuePairCollection(string attribute, object value)
        {
            this.Add(attribute, value);
        }

        public AttributeValuePairCollection(string attribute1, object value1, string attribute2, object value2)
        {
            this.internalList.Add(new AttributeValuePair(attribute1, value1));
            this.internalList.Add(new AttributeValuePair(attribute2, value2));
        }

        public AttributeValuePairCollection(string attribute1, object value1, string attribute2, object value2, string attribute3, object value3)
        {
            this.internalList.Add(new AttributeValuePair(attribute1, value1));
            this.internalList.Add(new AttributeValuePair(attribute2, value2));
            this.internalList.Add(new AttributeValuePair(attribute3, value3));
        }

        public AttributeValuePairCollection(string attribute1, object value1, string attribute2, object value2, string attribute3, object value3, string attribute4, object value4)
        {
            this.internalList.Add(new AttributeValuePair(attribute1, value1));
            this.internalList.Add(new AttributeValuePair(attribute2, value2));
            this.internalList.Add(new AttributeValuePair(attribute3, value3));
            this.internalList.Add(new AttributeValuePair(attribute4, value4));
        }

        public void Add(string attribute, object value)
        {
            this.internalList.Add(new AttributeValuePair(attribute, value));
        }

        public void Remove(string attribute)
        {
            this.internalList.RemoveAll(t => t.AttributeName == attribute);
        }

        public IEnumerator<AttributeValuePair> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }
    }
}
