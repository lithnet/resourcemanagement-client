using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Xml.Schema;
using System.Xml;

namespace Lithnet.ResourceManagement.Client
{
    public class ObjectTypeDefinition : IEnumerable<AttributeTypeDefinition>
    {
        public string SystemName { get; private set; }

        public string DisplayName { get; private set; }

        public string Description { get; private set; }

        private ReadOnlyCollection<AttributeTypeDefinition> Attributes { get; set; }

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

        public IEnumerator<AttributeTypeDefinition> GetEnumerator()
        {
            return this.Attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Attributes.GetEnumerator();
        }

        public override string ToString()
        {
            return this.SystemName;
        }
    }
}
