using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Xml.Schema;
using System.Xml;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Defines an object type from the Resource Management Service schema and its associated attributes
    /// </summary>
    public class ObjectTypeDefinition 
    {
        /// <summary>
        /// Gets the system name of the object type
        /// </summary>
        public string SystemName { get; private set; }

        /// <summary>
        /// Gets the display name of the object type
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the description of the object type
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// The read-only collection of attributes types associated with this object class
        /// </summary>
        public ReadOnlyCollection<AttributeTypeDefinition> Attributes { get; set; }

        /// <summary>
        /// Initializes a new instance of the ObjectTypeDefinition class
        /// </summary>
        /// <param name="complexType">The XML definition of the object type</param>
        internal ObjectTypeDefinition(XmlSchemaComplexType complexType)
        {
            XmlSchemaSequence sequence = complexType.Particle as XmlSchemaSequence;
                        
            if (sequence == null)
            {
                throw new InvalidOperationException("The object type definition did not contain any elements");
            }

            this.SystemName = complexType.Name;

            this.GetObjectTypeDetails(complexType.Annotation);

            List<AttributeTypeDefinition> attributes = new List<AttributeTypeDefinition>();

            XmlSchemaObjectCollection items = sequence.Items;

            foreach (XmlSchemaElement item in items.OfType<XmlSchemaElement>())
            {
                attributes.Add(new AttributeTypeDefinition(item));
            }

            this.Attributes = new ReadOnlyCollection<AttributeTypeDefinition>(attributes);
        }

        /// <summary>
        /// Gets the definition of an attribute by its name
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>An AttributeTypeDefinition for the specified attribute, or null if the attribute doesn't exist on the object type</returns>
        public AttributeTypeDefinition this[string attributeName]
        {
            get
            {
                if (this.Attributes == null)
                {
                    return null;
                }

                return this.Attributes.FirstOrDefault(t => t.SystemName == attributeName);
            }
        }

        /// <summary>
        /// Parses the basic details of the object type from its XML definition
        /// </summary>
        /// <param name="annotation">The XmlSchemaAnnotation containing the basic details of the object type</param>
        private void GetObjectTypeDetails(XmlSchemaAnnotation annotation)
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
        /// Gets the system name of the object type
        /// </summary>
        /// <returns>A string containing the system name of the object type</returns>
        public override string ToString()
        {
            return this.SystemName;
        }
    }
}
