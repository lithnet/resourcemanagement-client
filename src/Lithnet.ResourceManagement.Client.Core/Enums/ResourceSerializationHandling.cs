namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Defines the format that a resource is serialized in
    /// </summary>
    public enum ResourceSerializationHandling
    {
        /// <summary>
        /// A simple attribute/value pair. The resource is structured based on the resource's schema defintion. Requires the consuming system know the attributes at design time
        /// </summary>
        AttributeValuePairs = 0,

        /// <summary>
        /// The resource will be serialized with a fixed structure that does not change based on the schema. The structure is always known at design time.
        /// </summary>
        FixedStructure = 1
    }
}
