using System.Collections.Generic;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Defines a collection of attribute and value pairs
    /// </summary>
    public class AttributeValuePairCollection: IEnumerable<AttributeValuePair>
    {
        /// <summary>
        /// The internal list of value pairs
        /// </summary>
        private List<AttributeValuePair> internalList = new List<AttributeValuePair>();

        /// <summary>
        /// Initializes a new instance of the AttributeValuePairCollection class
        /// </summary>
        public AttributeValuePairCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AttributeValuePairCollection class
        /// </summary>
        /// <param name="attribute">The name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        public AttributeValuePairCollection(string attribute, object value)
        {
            this.Add(attribute, value);
        }

        /// <summary>
        /// Initializes a new instance of the AttributeValuePairCollection class
        /// </summary>
        /// <param name="attribute1">The name of the attribute in the first pair</param>
        /// <param name="value1">The value of the attribute in the first pair</param>
        /// <param name="attribute2">The name of the attribute in the second pair</param>
        /// <param name="value2">The value of the attribute in the second pair</param>
        public AttributeValuePairCollection(string attribute1, object value1, string attribute2, object value2)
        {
            this.internalList.Add(new AttributeValuePair(attribute1, value1));
            this.internalList.Add(new AttributeValuePair(attribute2, value2));
        }

        /// <summary>
        /// Initializes a new instance of the AttributeValuePairCollection class
        /// </summary>
        /// <param name="attribute1">The name of the attribute in the first pair</param>
        /// <param name="value1">The value of the attribute in the first pair</param>
        /// <param name="attribute2">The name of the attribute in the second pair</param>
        /// <param name="value2">The value of the attribute in the second pair</param>
        /// <param name="attribute3">The name of the attribute in the third pair</param>
        /// <param name="value3">The value of the attribute in the third pair</param>
        public AttributeValuePairCollection(string attribute1, object value1, string attribute2, object value2, string attribute3, object value3)
        {
            this.internalList.Add(new AttributeValuePair(attribute1, value1));
            this.internalList.Add(new AttributeValuePair(attribute2, value2));
            this.internalList.Add(new AttributeValuePair(attribute3, value3));
        }

        /// <summary>
        /// Initializes a new instance of the AttributeValuePairCollection class
        /// </summary>
        /// <param name="attribute1">The name of the attribute in the first pair</param>
        /// <param name="value1">The value of the attribute in the first pair</param>
        /// <param name="attribute2">The name of the attribute in the second pair</param>
        /// <param name="value2">The value of the attribute in the second pair</param>
        /// <param name="attribute3">The name of the attribute in the third pair</param>
        /// <param name="value3">The value of the attribute in the third pair</param>
        /// <param name="attribute4">The name of the attribute in the fourth pair</param>
        /// <param name="value4">The value of the attribute in the fourth pair</param>
        public AttributeValuePairCollection(string attribute1, object value1, string attribute2, object value2, string attribute3, object value3, string attribute4, object value4)
        {
            this.internalList.Add(new AttributeValuePair(attribute1, value1));
            this.internalList.Add(new AttributeValuePair(attribute2, value2));
            this.internalList.Add(new AttributeValuePair(attribute3, value3));
            this.internalList.Add(new AttributeValuePair(attribute4, value4));
        }

        /// <summary>
        /// Adds an attribute and value pair to the collection
        /// </summary>
        /// <param name="attribute">The name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        public void Add(string attribute, object value)
        {
            this.internalList.Add(new AttributeValuePair(attribute, value));
        }

        /// <summary>
        /// Removes all instances of the attribute in the collection
        /// </summary>
        /// <param name="attribute">The name of the attribute to remove</param>
        public void Remove(string attribute)
        {
            this.internalList.RemoveAll(t => t.AttributeName == attribute);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection
        /// </summary>
        /// <returns>An IEnumerator that iterates through the collection</returns>
        public IEnumerator<AttributeValuePair> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection
        /// </summary>
        /// <returns>An IEnumerator that iterates through the collection</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }
    }
}
