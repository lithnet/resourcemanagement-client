namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// /The type of value comparison to perform between an attribute and a value in an XPath query
    /// </summary>
    public enum ComparisonOperator
    {
        /// <summary>
        /// The attribute is equal to the specified value
        /// </summary>
        /// <remarks>
        /// This value is only supported when querying the following attribute types
        /// <list>
        /// <item>String</item>
        /// <item>Reference</item>
        /// <item>DateTime</item>
        /// <item>Integer</item>
        /// <item>Boolean</item>
        /// </list>
        /// </remarks>
        Equals,

        /// <summary>
        /// The attribute does not equal the specified value
        /// </summary>
        /// <remarks>
        /// This value is only supported when querying the following attribute types
        /// <list>
        /// <item>String</item>
        /// <item>Reference</item>
        /// <item>DateTime</item>
        /// <item>Integer</item>
        /// <item>Boolean</item>
        /// </list>
        /// </remarks>
        NotEquals,

        /// <summary>
        /// The attribute is greater than the specified value
        /// </summary>
        /// <remarks>
        /// This value is only supported when querying the following attribute types
        /// <list>
        /// <item>DateTime</item>
        /// <item>Integer</item>
        /// </list>
        /// </remarks>
        GreaterThan,

        /// <summary>
        /// The attribute is greater than or equal to the specified value
        /// </summary>
        /// <remarks>
        /// This value is only supported when querying the following attribute types
        /// <list>
        /// <item>DateTime</item>
        /// <item>Integer</item>
        /// </list>
        /// </remarks>
        GreaterThanOrEquals,

        /// <summary>
        /// The attribute is less than the specified value
        /// </summary>
        /// <remarks>
        /// This value is only supported when querying the following attribute types
        /// <list>
        /// <item>DateTime</item>
        /// <item>Integer</item>
        /// </list>
        /// </remarks>
        LessThan,

        /// <summary>
        /// The attribute is less than or equal to the specified value
        /// </summary>
        /// <remarks>
        /// This value is only supported when querying the following attribute types
        /// <list>
        /// <item>DateTime</item>
        /// <item>Integer</item>
        /// </list>
        /// </remarks>
        LessThanOrEquals,

        /// <summary>
        /// The attribute has a value
        /// </summary>
        /// <remarks>
        /// This value is only supported when querying the following attribute types
        /// <list>
        /// <item>String</item>
        /// <item>Reference</item>
        /// <item>DateTime</item>
        /// <item>Integer</item>
        /// <item>Boolean</item>
        /// </list>
        /// </remarks>
        IsPresent,

        /// <summary>
        /// The attribute does not have a value
        /// </summary>
        /// <remarks>
        /// This value is only supported when querying the following attribute types
        /// <list>
        /// <item>String</item>
        /// <item>Reference</item>
        /// <item>DateTime</item>
        /// <item>Integer</item>
        /// <item>Boolean</item>
        /// </list>
        /// </remarks>
        IsNotPresent,

        /// <summary>
        /// The attribute contains the specified text
        /// </summary>
        /// <remarks>
        /// This value is only supported when querying the following attribute types
        /// <list>
        /// <item>String</item>
        /// </list>
        /// </remarks>
        Contains,

        /// <summary>
        /// The attribute starts with the specified text
        /// </summary>
        /// <remarks>
        /// This value is only supported when querying the following attribute types
        /// <list>
        /// <item>String</item>
        /// </list>
        /// </remarks>
        StartsWith,

        /// <summary>
        /// The attribute ends with the specified test
        /// </summary>
        /// <remarks>
        /// This value is only supported when querying the following attribute types
        /// <list>
        /// <item>String</item>
        /// </list>
        /// </remarks>
        EndsWith
    }
}
