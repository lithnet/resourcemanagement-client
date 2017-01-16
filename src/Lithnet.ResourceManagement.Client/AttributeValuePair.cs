namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Represents a single attribute and value pair
    /// </summary>
    public class AttributeValuePair
    {
        /// <summary>
        /// The name of the attribute
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// The value of the attribute
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the AttributeValuePair class
        /// </summary>
        public AttributeValuePair()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AttributeValuePair class
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        public AttributeValuePair(string attributeName, object value)
        {
            this.AttributeName = attributeName;
            this.Value = value;
        }
    }
}
