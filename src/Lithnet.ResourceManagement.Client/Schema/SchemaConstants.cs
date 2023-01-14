using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Lithnet.ResourceManagement.Client
{
    public static class SchemaConstants
    {
        /// <summary>
        /// Gets the list of attributes that are considered mandatory for all object classes
        /// </summary>
        public static ReadOnlyCollection<string> MandatoryAttributes { get; }

        /// <summary>
        /// Gets a list of attributes that are computed and cannot be changed
        /// </summary>
        public static ReadOnlyCollection<string> ComputedAttributes { get; }

        /// <summary>
        /// A list of elements contained in the XML schema from the Resource Management Service that should be ignored. Certain complex types are present
        /// in the XML definition that do not represent object classes
        /// </summary>
        internal static List<string> ElementsToIgnore { get; }

        static SchemaConstants()
        {
            MandatoryAttributes = new ReadOnlyCollection<string>(new List<string>() { AttributeNames.ObjectType, AttributeNames.ObjectID });

            ComputedAttributes = new ReadOnlyCollection<string>(new List<string>
            {
                "Creator",
                "CreatedTime",
                "ExpectedRulesList",
                "DetectedRulesList",
                "DeletedTime",
                "ResourceTime",
                "ComputedMember",
                "ComputedActor"
            });

            ElementsToIgnore = new List<string>
            {
                "ReferenceType",
                "BinaryCollectionType",
                "DateTimeCollectionType",
                "IntegerCollectionType",
                "ReferenceCollectionType",
                "StringCollectionType",
                "TextCollectionType"
            };
        }
    }
}
