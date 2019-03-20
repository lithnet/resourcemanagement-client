namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Defines a pending change to an attribute value
    /// </summary>
    public class AttributeValueChange
    {
        /// <summary>
        /// Initializes a new instance of the AttributeValueChange class
        /// </summary>
        /// <param name="changeType">The modification type to apply</param>
        /// <param name="value">The value that is changing</param>
        internal AttributeValueChange(ModeType changeType, object value)
        {
            this.ChangeType = changeType;
            this.Value = value;
        }

        /// <summary>
        /// Gets the modification type for this change
        /// </summary>
        public ModeType ChangeType { get; private set; }

        /// <summary>
        /// Gets the value that is changing
        /// </summary>
        public object Value { get; private set; }
    }
}
