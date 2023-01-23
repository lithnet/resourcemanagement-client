using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using System.Xml;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Defines an attribute type in the Resource Management Service schema
    /// </summary>
    public class AttributeTypeDefinition
    {
        /// <summary>
        /// Gets a value that indicates whether the attribute is required
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// Gets the data type of the attribute
        /// </summary>
        public AttributeType Type { get; private set; }

        /// <summary>
        /// Gets the system name of the attribute
        /// </summary>
        public string SystemName { get; private set; }

        /// <summary>
        /// Gets the display name of the attribute
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the description of the attribute
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this attribute is multivalued or single-valued
        /// </summary>
        public bool IsMultivalued { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this attribute is read only
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Gets the regular expression valued to validate the correctness of this attribute
        /// </summary>
        public string Regex { get; private set; }

       /// <summary>
        /// Gets or sets the list of attribute names that are always read-only in the schema
        /// </summary>
        private static List<string> ReadOnlyAttributeNames { get; set; }

        /// <summary>
        /// Initializes the static instance of the AttributeTypeDefinition class
        /// </summary>
        static AttributeTypeDefinition()
        {
            ReadOnlyAttributeNames = new List<string>
            {
                AttributeNames.ObjectID,
                "Creator",
                "CreatedTime",
                "ExpectedRulesList",
                "DetectedRulesList",
                "DeletedTime",
                "ResourceTime",
                "ComputedMember",
                "ComputedActor"
            };
        }

        /// <summary>
        /// Initializes an instance of the AttributeTypeDefinititon class
        /// </summary>
        /// <param name="systemName">The system name of the attribute</param>
        /// <param name="type">The data type of the attribute</param>
        /// <param name="isMultivalued">A value indicating if the attribute is multivalued or single-valued</param>
        /// <param name="isReadOnly">A value indicating if the attribute is read only</param>
        /// <param name="isRequired">A value indicating if the attribute is required</param>
        internal AttributeTypeDefinition(string systemName, AttributeType type, bool isMultivalued, bool isReadOnly, bool isRequired)
        {
            this.SystemName = systemName;
            this.DisplayName = systemName;
            this.IsMultivalued = isMultivalued;
            this.IsReadOnly = isReadOnly;
            this.IsRequired = isRequired;
            this.Type = type;
        }

        /// <summary>
        /// Initializes an instance of the AttributeTypeDefinition class from its XML schema definition
        /// </summary>
        /// <param name="schemaObject">The XML definition of the attribute</param>
        internal AttributeTypeDefinition(XmlSchemaElement schemaObject)
        {
            XmlSchemaElement element = schemaObject as XmlSchemaElement;

            if (element == null)
            {
                throw new InvalidOperationException("Unexpected element type in schema");
            }

            this.SystemName = schemaObject.Name;
            this.GetAttributeDetails(schemaObject.Annotation);

            if (ReadOnlyAttributeNames.Contains(this.SystemName))
            {
                this.IsReadOnly = true;
            }

            if (element.MinOccurs > 0)
            {
                this.IsRequired = true;
            }

            if (schemaObject.SchemaType == null)
            {
                // A complex schema type
                if (schemaObject.SchemaTypeName == null)
                {
                    throw new InvalidOperationException("The schema was invalid as the custom type name was not present");
                }

                this.LoadComplexType(schemaObject.SchemaTypeName);
            }
            else
            {
                // A simple schema type
                XmlSchemaSimpleType simpleType = schemaObject.SchemaType as XmlSchemaSimpleType;

                if (simpleType == null)
                {
                    throw new InvalidOperationException("The expected simple type was not found in the schema");
                }

                this.LoadSimpleType(simpleType);
            }
        }

        /// <summary>
        /// Gets the basic details of the attribute from the XML schema annotation object
        /// </summary>
        /// <param name="annotation">The XML schema annotation containing the basic details of the attribute</param>
        private void GetAttributeDetails(XmlSchemaAnnotation annotation)
        {
            XmlSchemaObjectCollection items = annotation.Items;

            foreach (XmlSchemaAppInfo info in items.OfType<XmlSchemaAppInfo>())
            {
                foreach (XmlElement element in info.Markup.OfType<XmlElement>())
                {
                    if (element.LocalName == "Description")
                    {
                        this.Description = element.InnerText;
                    }
                    else if (element.LocalName == "DisplayName")
                    {
                        this.DisplayName = element.InnerText;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the definition of the attribute from an XML schema simple type
        /// </summary>
        /// <param name="type">The XML schema simple type representing this attribute</param>
        private void LoadSimpleType(XmlSchemaSimpleType type)
        {
            XmlSchemaSimpleTypeRestriction restriction = type.Content as XmlSchemaSimpleTypeRestriction;

            if (((restriction == null) || (null == restriction.BaseTypeName)) || (restriction.BaseTypeName.Name == null))
            {
                throw new InvalidOperationException("The schema attribute had an invalid or missing restriction");
            }

            XmlSchemaObjectCollection facets = restriction.Facets;

            if (restriction.BaseTypeName.Name == "string")
            {
                this.Type = AttributeType.String;

                if (facets.Count != 1)
                {
                    throw new InvalidOperationException("The number of restrictions on the attribute type was invalid");
                }

                XmlSchemaPatternFacet facet = facets[0] as XmlSchemaPatternFacet;

                if (facet.Value != ".{0,448}")
                {
                    this.Regex = facet.Value;
                }

                if (this.Regex != ".{0,448}" && !HasStringAnnotation(type.Annotation))
                {
                    this.Type = AttributeType.Text;
                }

                this.IsMultivalued = false;
            }
            else if (restriction.BaseTypeName.Name == "integer")
            {
                this.Type = AttributeType.Integer;
                this.IsMultivalued = false;
            }
            else
            {
                throw new InvalidOperationException("The attribute type was unknown");
            }
        }

        /// <summary>
        /// Gets the definition of the attribute from an XML complex type
        /// </summary>
        /// <param name="type">The XML schema complex type representing this attribute</param>
        private void LoadComplexType(XmlQualifiedName type)
        {
            string typeName = type.Name;

            switch (typeName)
            {
                case "BinaryCollectionType":
                    this.Type = AttributeType.Binary;
                    this.IsMultivalued = true;
                    break;

                case "base64Binary":
                    this.Type = AttributeType.Binary;
                    this.IsMultivalued = false;
                    break;

                case "boolean":
                    this.Type = AttributeType.Boolean;
                    this.IsMultivalued = false;
                    break;

                case "DateTimeCollectionType":
                    this.Type = AttributeType.DateTime;
                    this.IsMultivalued = true;
                    break;

                case "dateTime":
                    this.Type = AttributeType.DateTime;
                    this.IsMultivalued = false;
                    break;

                case "IntegerCollectionType":
                    this.Type = AttributeType.Integer;
                    this.IsMultivalued = true;
                    break;

                case "integer":
                    this.Type = AttributeType.Integer;
                    this.IsMultivalued = false;
                    break;

                case "ReferenceType":
                    this.Type = AttributeType.Reference;
                    this.IsMultivalued = false;
                    break;

                case "ReferenceCollectionType":
                    this.Type = AttributeType.Reference;
                    this.IsMultivalued = true;
                    break;

                case "string":
                    // Note: indexable strings appear as custom types with a length restriction, whereas Text data type are a string
                    // with no restriction
                    this.Type = AttributeType.Text;
                    this.IsMultivalued = false;
                    break;

                case "StringCollectionType":
                    this.Type = AttributeType.String;
                    this.IsMultivalued = true;
                    break;

                case "TextCollectionType":
                    this.Type = AttributeType.Text;
                    this.IsMultivalued = true;
                    break;

                default:
                    throw new ArgumentException("Unknown attribute type in schema " + typeName);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the XML schema annotation contains an annotation describing this attribute as a string type
        /// </summary>
        /// <param name="annotation">The XML schema annotation to inspect</param>
        /// <returns></returns>
        private static bool HasStringAnnotation(XmlSchemaAnnotation annotation)
        {
            XmlSchemaObjectCollection items = annotation.Items;

            foreach (XmlSchemaAppInfo appInfo in items.OfType<XmlSchemaAppInfo>())
            {
                foreach (XmlNode node in appInfo.Markup)
                {
                    XmlElement element = node as XmlElement;

                    if (element == null)
                    {
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(element.LocalName))
                    {
                        return false;
                    }

                    if (element.LocalName == "DataType" && element.InnerText == "String")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the system name of this attribute
        /// </summary>
        /// <returns>A string representing the system name of the attribute</returns>
        public override string ToString()
        {
            return this.SystemName;
        }
    }
}
