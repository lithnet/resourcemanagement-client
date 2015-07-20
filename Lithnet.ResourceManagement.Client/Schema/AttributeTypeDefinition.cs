using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using Lithnet.ResourceManagement.Client;
using System.Xml;

namespace Lithnet.ResourceManagement.Client
{
    public class AttributeTypeDefinition
    {
        public bool IsRequired { get; private set; }

        public AttributeType Type { get; private set; }

        public string SystemName { get; private set; }

        public string DisplayName { get; private set; }

        public string Description { get; private set; }

        public bool IsMultivalued { get; private set; }

        public bool IsReadOnly { get; private set; }

        public string Regex { get; private set; }

        private static List<string> ReadOnlyAttributeNames { get; set; }

        static AttributeTypeDefinition()
        {
            AttributeTypeDefinition.ReadOnlyAttributeNames = new List<string>();
            AttributeTypeDefinition.ReadOnlyAttributeNames.Add("ObjectID");
            AttributeTypeDefinition.ReadOnlyAttributeNames.Add("Creator");
            AttributeTypeDefinition.ReadOnlyAttributeNames.Add("CreatedTime");
            AttributeTypeDefinition.ReadOnlyAttributeNames.Add("ExpectedRulesList");
            AttributeTypeDefinition.ReadOnlyAttributeNames.Add("DetectedRulesList");
            AttributeTypeDefinition.ReadOnlyAttributeNames.Add("DeletedTime");
            AttributeTypeDefinition.ReadOnlyAttributeNames.Add("ResourceTime");
            AttributeTypeDefinition.ReadOnlyAttributeNames.Add("ComputedMember");
            AttributeTypeDefinition.ReadOnlyAttributeNames.Add("ComputedActor");
        }

        internal AttributeTypeDefinition(string systemName, AttributeType type, bool isMultivalued, bool isReadOnly, bool isRequired)
        {
            this.SystemName = systemName;
            this.DisplayName = systemName;
            this.IsMultivalued = isMultivalued;
            this.IsReadOnly = isReadOnly;
            this.IsRequired = isRequired;
            this.Type = type;
        }

        internal AttributeTypeDefinition(XmlSchemaElement schemaObject)
        {
            XmlSchemaElement element = schemaObject as XmlSchemaElement;

            if (element == null)
            {
                throw new InvalidOperationException("Unexpected element type in schema");
            }

            this.SystemName = schemaObject.Name;
            this.GetAttributeDetails(schemaObject.Annotation);

            if (AttributeTypeDefinition.ReadOnlyAttributeNames.Contains(this.SystemName))
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

        public override string ToString()
        {
            return this.SystemName;
        }
    }
}
