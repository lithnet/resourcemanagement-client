using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Contains a collection of attributes and values
    /// </summary>
    public class AttributeValueCollection : IEnumerable<AttributeValue>
    {
        /// <summary>
        /// The internal dictionary of attribute and value paiirs
        /// </summary>
        private Dictionary<string, AttributeValue> internalValues;

        /// <summary>
        /// Initializes a new instance of the AttributeValuesCollection class
        /// </summary>
        internal AttributeValueCollection()
        {
            this.internalValues = new Dictionary<string, AttributeValue>();
        }

        /// <summary>
        /// Gets an attribute value collection
        /// </summary>
        /// <param name="attributeName">The name of the attribute to get</param>
        /// <returns>An <c ref="AttributeValue">AttributeValue</c> object</returns>
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

        /// <summary>
        /// Adds a new attribute value object to the internal dictionary
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="value">The attribute value to add</param>
        internal void Add(string attributeName, AttributeValue value)
        {
            if (attributeName == null)
            {
                throw new ArgumentNullException("attributeName");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.internalValues.Add(attributeName, value);
        }

        /// <summary>
        /// Adds a new attribute value object to the internal dictionary
        /// </summary>
        /// <param name="value">The attribute value to add</param>
        internal void Add(AttributeValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.AttributeName == null)
            {
                throw new InvalidOperationException("The AttributeValue object did not have an attribute associated with it");
            }

            this.internalValues.Add(value.AttributeName, value);
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains a specific attribute
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>True if the collection contains the specified attribute</returns>
        public bool ContainsAttribute(string attributeName)
        {
            return this.internalValues.ContainsKey(attributeName);
        }

        /// <summary>
        /// Gets an enumerator that iterates though the AttributeValueCollection
        /// </summary>
        /// <returns>Returns an enumerator that iterates though the AttributeValueCollection</returns>
        public System.Collections.Generic.IEnumerator<AttributeValue> GetEnumerator()
        {
            return this.internalValues.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates though the AttributeValueCollection
        /// </summary>
        /// <returns>Returns an enumerator that iterates though the AttributeValueCollection</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.internalValues.Values.GetEnumerator();
        }
    }
}
