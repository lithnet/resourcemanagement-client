using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ResourceManagement.WebServices;
using Microsoft.ResourceManagement.WebServices.Client;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;
using System.Xml.Serialization;
using System.Xml;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable]
    [KnownType(typeof(List<string>))]
    public class ResourceObject : ISerializable
    {
        private ResourceManagementClient client;

        private List<string> AttributesToIgnore;

        private AttributeValueCollection attributes;

        private ResourceManagementClient Client
        {
            get
            {
                if (this.client == null)
                {
                    this.client = new ResourceManagementClient();
                }

                return this.client;
            }
        }

        public string ObjectTypeName
        {
            get
            {
                return this.ObjectType.SystemName;
            }
        }

        internal bool IsPlaceHolder { get; private set; }

        internal bool IsDeleted { get; set; }

        public OperationType ModificationType { get; private set; }

        public ObjectTypeDefinition ObjectType { get; private set; }

        public AttributeValueCollection Attributes
        {
            get
            {
                return this.attributes;
            }
        }

        public UniqueIdentifier ObjectID
        {
            get
            {
                if (this.attributes.ContainsAttribute(AttributeNames.ObjectID))
                {
                    return this.attributes[AttributeNames.ObjectID].ReferenceValue;
                }
                else
                {
                    return new UniqueIdentifier();
                }
            }
        }

        public string DisplayName
        {
            get
            {
                if (this.attributes.ContainsAttribute(AttributeNames.DisplayName))
                {
                    return this.attributes[AttributeNames.DisplayName].StringValue;
                }
                else
                {
                    return null;
                }
            }
        }

        private ResourceObject(OperationType opType, ResourceManagementClient client)
        {
            this.ModificationType = opType;
            this.attributes = new AttributeValueCollection();
            this.AttributesToIgnore = new List<string>();
            AttributesToIgnore.Add("Creator");
            AttributesToIgnore.Add("CreatedTime");
            AttributesToIgnore.Add("ExpectedRulesList");
            AttributesToIgnore.Add("DetectedRulesList");
            AttributesToIgnore.Add("DeletedTime");
            AttributesToIgnore.Add("ResourceTime");
            AttributesToIgnore.Add("ComputedMember");
            AttributesToIgnore.Add("ComputedActor");
            this.client = client;
        }

        protected ResourceObject(SerializationInfo info, StreamingContext context)
        {
            throw new NotSupportedException();
        }

        internal ResourceObject(string type, ResourceManagementClient client)
            : this(OperationType.Create, client)
        {
            if (!Schema.ObjectTypes.ContainsKey(type))
            {
                throw new ArgumentException(string.Format("Unknown object type {0}", type));
            }

            this.IsPlaceHolder = true;

            this.ObjectType = Schema.ObjectTypes[type];
            this.AddRemainingAttributesFromSchema();

            this.attributes[AttributeNames.ObjectType].SetValue(type);
        }

        internal ResourceObject(string type, UniqueIdentifier id, ResourceManagementClient client)
            : this(OperationType.Update, client)
        {
            if (!Schema.ObjectTypes.ContainsKey(type))
            {
                throw new ArgumentException(string.Format("Unknown object type {0}", type));
            }

            this.ObjectType = Schema.ObjectTypes[type];
            this.AddRemainingAttributesFromSchema();
            this.IsPlaceHolder = true;

            this.attributes[AttributeNames.ObjectType].SetValue(type);
        }

        internal ResourceObject(IEnumerable<XmlElement> objectElements, ResourceManagementClient client)
            : this(OperationType.Update, client)
        {
            this.PopulateResourceFromPartialResponse(objectElements);
        }

        internal ResourceObject(XmlDictionaryReader reader, ResourceManagementClient client)
            : this(OperationType.Update, client)
        {
            this.PopulateResourceFromFullObject(reader);
        }

        internal ResourceObject(XmlElement element, ResourceManagementClient client)
            : this(OperationType.Update, client)
        {
            this.PopulateResourceFromFragment(element);
        }

        public Dictionary<string, List<AttributeValueChange>> PendingChanges
        {
            get
            {
                Dictionary<string, List<AttributeValueChange>> changeList = new Dictionary<string, List<AttributeValueChange>>();

                foreach (AttributeValue attributeValue in this.attributes)
                {
                    IList<AttributeValueChange> changes = attributeValue.ValueChanges;

                    if (changes.Count > 0)
                    {
                        changeList.Add(attributeValue.AttributeName, new List<AttributeValueChange>());

                        foreach (var change in changes)
                        {
                            changeList[attributeValue.AttributeName].Add(change);
                        }
                    }
                }

                return changeList;
            }
        }

        public void UndoChanges()
        {
            foreach (var attributeValue in this.attributes)
            {
                attributeValue.UndoChanges();
            }
        }

        public void Save(bool refresh)
        {
            this.Save();

            if (refresh)
            {
                this.Refresh();
            }
        }

        public void Save()
        {
            switch (this.ModificationType)
            {
                case OperationType.Create:
                    this.Client.CreateResource(this);
                    break;

                case OperationType.Update:
                    this.Client.PutResource(this);
                    break;

                case OperationType.Delete:
                    if (this.IsDeleted)
                    {
                        throw new InvalidOperationException("The object has already been deleted");
                    }

                    this.Client.DeleteResource(this);
                    break;

                case OperationType.None:
                default:
                    throw new InvalidOperationException("The operation type was not set or is unknown");
            }

            this.CommitChanges();

            if (this.ModificationType == OperationType.Create)
            {
                this.ModificationType = OperationType.Update;
            }

            this.IsPlaceHolder = false;
        }

        public bool HasValue(string name)
        {
            if (!this.attributes.ContainsAttribute(name))
            {
                return false;
            }

            return !attributes[name].IsNull;
        }

        public void Refresh()
        {
            if (this.IsPlaceHolder)
            {
                throw new InvalidOperationException("Cannot refresh a placeholder object. Call Save() to submit the changes to the resource management service");
            }

            if (this.IsDeleted)
            {
                throw new InvalidOperationException("Cannot refresh the object as it has been deleted");
            }

            XmlDictionaryReader reader = this.Client.RefreshResource(this);

            this.PopulateResourceFromFullObject(reader);
            this.IsPlaceHolder = false;
            this.ModificationType = OperationType.Update;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (AttributeValue value in this.attributes)
            {
                sb.AppendLine(string.Format("{0}: {1}", value.AttributeName, value.ToString()));
            }

            return sb.ToString();
        }

        public Dictionary<string, IList<string>> GetSerializationValues()
        {
            Dictionary<string, IList<string>> values = new Dictionary<string, IList<string>>();

            foreach (AttributeValue kvp in this.attributes)
            {
                if (!kvp.IsNull)
                {
                    values.Add(kvp.AttributeName, kvp.GetSerializationValues());
                }
            }

            return values;
        }

        internal void CompleteCreateOperation(UniqueIdentifier id)
        {
            if (this.ModificationType != OperationType.Create)
            {
                throw new InvalidOperationException();
            }

            AttributeTypeDefinition objectID = this.ObjectType[AttributeNames.ObjectID];
            if (!this.attributes.ContainsAttribute(AttributeNames.ObjectID))
            {
                this.attributes.Add(new AttributeValue(objectID, id));
            }
            else
            {
                this.attributes[AttributeNames.ObjectID] = new AttributeValue(objectID, id);
            }

            this.CommitChanges();

            this.ModificationType = OperationType.Update;
            this.IsPlaceHolder = false;
        }

        internal void CommitChanges()
        {
            foreach (AttributeValue attributeValues in this.attributes)
            {
                attributeValues.Commit();
            }
        }

        internal List<PutFragmentType> GetPutFragements()
        {
            List<PutFragmentType> fragments = new List<PutFragmentType>();
            foreach (KeyValuePair<string, List<AttributeValueChange>> kvp in this.PendingChanges)
            {
                AttributeTypeDefinition type = this.ObjectType[kvp.Key];

                if (this.AttributesToIgnore.Contains(type.SystemName))
                {
                    continue;
                }

                foreach (AttributeValueChange change in kvp.Value)
                {
                    string value = null;

                    if (change.Value != null)
                    {
                        switch (type.Type)
                        {
                            case AttributeType.Binary:
                                value = Convert.ToBase64String((byte[])change.Value);
                                break;

                            case AttributeType.DateTime:
                                value = ((DateTime)change.Value).ToResourceManagementServiceDateFormat();
                                break;

                            case AttributeType.Integer:
                            case AttributeType.Boolean:
                            case AttributeType.String:
                            case AttributeType.Text:
                                value = change.Value.ToString();
                                break;

                            case AttributeType.Reference:
                                value = ((UniqueIdentifier)change.Value).ToString();
                                break;

                            case AttributeType.Unknown:
                            default:
                                throw new ArgumentException(string.Format("Unknown value type {0}", change.Value.GetType().Name));
                        }
                    }

                    PutFragmentType fragment = new PutFragmentType(kvp.Key, change.ChangeType, kvp.Key, null, false, value);
                    fragments.Add(fragment);
                }
            }

            return fragments;
        }

        private void SetAttributeValues(Dictionary<string, List<string>> values)
        {
            this.attributes = new AttributeValueCollection();

            foreach (KeyValuePair<string, List<string>> kvp in values)
            {
                if (kvp.Value.Count == 0)
                {
                    continue;
                }

                string attributeName = kvp.Key;
                AttributeTypeDefinition d = this.ObjectType[attributeName];

                if (!d.IsMultivalued && kvp.Value.Count > 1)
                {
                    throw new InvalidOperationException("The attribute {0} is listed in the schema as a multivalued attribute, but more than one value was returned");
                }

                if (d.IsMultivalued)
                {
                    this.attributes.Add(d.SystemName, new AttributeValue(d, kvp.Value));
                }
                else
                {
                    this.attributes.Add(d.SystemName, new AttributeValue(d, kvp.Value.First()));
                }
            }

            this.AddRemainingAttributesFromSchema();
        }

        private void AddRemainingAttributesFromSchema()
        {
            foreach (AttributeTypeDefinition attributeDefinition in this.ObjectType)
            {
                if (!this.attributes.ContainsAttribute(attributeDefinition.SystemName))
                {
                    this.attributes.Add(attributeDefinition.SystemName, new AttributeValue(attributeDefinition));
                }
            }
        }

        private void PopulateResourceFromFullObject(XmlDictionaryReader reader)
        {
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();

            reader.MoveToStartElement();

            string objectTypeName = reader.LocalName;
            this.ObjectType = Schema.ObjectTypes[objectTypeName];

            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                if (!values.ContainsKey(reader.LocalName))
                {
                    values.Add(reader.LocalName, new List<string>());
                }

                string value = reader.ReadString();
                if (!string.IsNullOrEmpty(value))
                {
                    values[reader.LocalName].Add(value);
                }
            }

            this.SetAttributeValues(values);
        }

        private void PopulateResourceFromFragment(XmlElement element)
        {
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();

            string objectTypeName = element.LocalName;
            this.ObjectType = Schema.ObjectTypes[objectTypeName];


            foreach (XmlElement child in element.ChildNodes.OfType<XmlElement>())
            {
                if (!values.ContainsKey(child.LocalName))
                {
                    values.Add(child.LocalName, new List<string>());
                }

                string value = child.InnerText;

                if (!string.IsNullOrEmpty(value))
                {
                    values[child.LocalName].Add(value);
                }
            }

            this.SetAttributeValues(values);
        }

        private void PopulateResourceFromPartialResponse(IEnumerable<XmlElement> objectElements)
        {
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();

            foreach (XmlElement partialAttributeElement in objectElements.Where(t => t.LocalName == "PartialAttribute"))
            {
                foreach (XmlElement attributeElement in partialAttributeElement.ChildNodes.OfType<XmlElement>())
                {
                    if (!values.ContainsKey(attributeElement.LocalName))
                    {
                        values.Add(attributeElement.LocalName, new List<string>());
                    }

                    values[attributeElement.LocalName].Add(attributeElement.InnerText);
                }
            }

            if (values.ContainsKey(AttributeNames.ObjectType))
            {
                string objectTypeName = values[AttributeNames.ObjectType].First();
                ObjectTypeDefinition objectType = Schema.ObjectTypes[objectTypeName];
                this.ObjectType = objectType;
            }
            else
            {
                throw new ArgumentException("No object type was specified in the response");
            }

            this.SetAttributeValues(values);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (KeyValuePair<string, IList<string>> kvp in this.GetSerializationValues())
            {
                if (kvp.Value.Count > 1)
                {
                    info.AddValue(kvp.Key, kvp.Value, typeof(List<string>));
                }
                else if (kvp.Value.Count == 1)
                {
                    info.AddValue(kvp.Key, kvp.Value.First(), typeof(string));
                }
                else
                {
                    continue;
                }
            }
        }
    }
}