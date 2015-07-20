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

namespace Lithnet.ResourceManagement.Client
{
    [Serializable]
    [KnownType(typeof(List<string>))]
    public class ResourceObject : ISerializable
    {
        internal bool IsDeleted { get; set; }

        private ResourceManagementClient client;

        private List<string> AttributesToIgnore;

        public string ObjectTypeName
        {
            get
            {
                return this.ObjectType.SystemName;
            }
        }

        public OperationType ModificationType { get; private set; }

        public ObjectTypeDefinition ObjectType { get; private set; }

        private Dictionary<string, AttributeValue> attributes;

        public UniqueIdentifier ObjectID
        {
            get
            {
                if (this.attributes.ContainsKey("ObjectID"))
                {
                    return this.attributes["ObjectID"].Value as UniqueIdentifier;
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
                if (this.attributes.ContainsKey("DisplayName"))
                {
                    return this.attributes["DisplayName"].GetValue<string>();
                }
                else
                {
                    return null;
                }
            }
        }

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

        private ResourceObject(OperationType opType)
        {
            this.ModificationType = opType;
            this.attributes = new Dictionary<string, AttributeValue>();
            this.AttributesToIgnore = new List<string>();
            AttributesToIgnore.Add("Creator");
            AttributesToIgnore.Add("CreatedTime");
            AttributesToIgnore.Add("ExpectedRulesList");
            AttributesToIgnore.Add("DetectedRulesList");
            AttributesToIgnore.Add("DeletedTime");
            AttributesToIgnore.Add("ResourceTime");
            AttributesToIgnore.Add("ComputedMember");
            AttributesToIgnore.Add("ComputedActor");
        }

        protected ResourceObject(SerializationInfo info, StreamingContext context)
        {
            //throw new NotSupportedException();
        }

        internal ResourceObject(string type)
            : this(OperationType.Create)
        {
            if (!Schema.ObjectTypes.ContainsKey(type))
            {
                throw new ArgumentException(string.Format("Unknown object type {0}", type));
            }

            this.ObjectType = Schema.ObjectTypes[type];
            this.PopulateAttributeDefinitions();

            this.attributes["ObjectType"].SetValue(type);
        }

        internal ResourceObject(string type, UniqueIdentifier id)
            : this(OperationType.Update)
        {
            if (!Schema.ObjectTypes.ContainsKey(type))
            {
                throw new ArgumentException(string.Format("Unknown object type {0}", type));
            }

            this.ObjectType = Schema.ObjectTypes[type];
            this.PopulateAttributeDefinitions();

            this.attributes["ObjectType"].SetValue(type);
        }

        internal ResourceObject(IEnumerable<XmlElement> objectElements)
            : this(OperationType.Update)
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

            if (values.ContainsKey("ObjectType"))
            {
                string objectTypeName = values["ObjectType"].First();
                ObjectTypeDefinition objectType = Schema.ObjectTypes[objectTypeName];
                this.ObjectType = objectType;
            }
            else
            {
                throw new ArgumentException("No object type was specified in the response");
            }

            this.ProcessIncomingAttributeValues(values);
        }

        private void ProcessIncomingAttributeValues(Dictionary<string, List<string>> values)
        {
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

            this.PopulateAttributeDefinitions();
        }

        internal ResourceObject(XmlDictionaryReader reader)
            : this(OperationType.Update)
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

            this.ProcessIncomingAttributeValues(values);
        }

        internal ResourceObject(XmlElement element)
            : this(OperationType.Update)
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

            this.ProcessIncomingAttributeValues(values);
        }

        public AttributeValue this[string attributeName]
        {
            get
            {
                return this.attributes[attributeName];
            }
        }

        public Dictionary<string, List<AttributeValueChange>> PendingChanges
        {
            get
            {
                Dictionary<string, List<AttributeValueChange>> changeList = new Dictionary<string, List<AttributeValueChange>>();

                foreach (var attribute in this.attributes.Values)
                {
                    IList<AttributeValueChange> changes = attribute.ValueChanges;

                    if (changes.Count > 0)
                    {
                        changeList.Add(attribute.AttributeName, new List<AttributeValueChange>());

                        foreach (var change in changes)
                        {
                            changeList[attribute.AttributeName].Add(change);
                        }
                    }
                }

                return changeList;
            }
        }

        public void UndoChanges()
        {
            foreach (var attribute in this.attributes.Values)
            {
                attribute.UndoChanges();
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

                    this.client.DeleteResource(this);
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
        }

        public bool HasValue(string name)
        {
            if (!this.attributes.ContainsKey(name))
            {
                return false;
            }

            return !attributes[name].IsNull;
        }

        public T GetValue<T>(string name)
        {
            if (!this.attributes.ContainsKey(name))
            {
                return default(T);
            }

            return this.attributes[name].GetValue<T>();
        }

        public IList<T> GetValues<T>(string name)
        {
            if (!this.attributes.ContainsKey(name))
            {
                return new List<T>();
            }

            return this.attributes[name].GetValues<T>();
        }

        public void AddValue(string name, byte[] value)
        {
            this.attributes[name].AddValue(value);
        }

        public void AddValue(string name, DateTime value)
        {
            this.attributes[name].AddValue(value);
        }

        public void AddValue(string name, int value)
        {
            this.attributes[name].AddValue(value);
        }

        public void AddValue(string name, long value)
        {
            this.attributes[name].AddValue(value);
        }

        public void AddValue(string name, string value)
        {
            this.attributes[name].AddValue(value);
        }

        public void AddValue(string name, UniqueIdentifier value)
        {
            this.attributes[name].AddValue(value);
        }

        public void AddValue(string name, Guid value)
        {
            this.attributes[name].AddValue(value);
        }

        public void RemoveValue(string name, byte[] value)
        {
            this.attributes[name].RemoveValue(value);
        }

        public void RemoveValue(string name, DateTime value)
        {
            this.attributes[name].RemoveValue(value);
        }

        public void RemoveValue(string name, int value)
        {
            this.attributes[name].RemoveValue(value);
        }

        public void RemoveValue(string name, long value)
        {
            this.attributes[name].RemoveValue(value);
        }

        public void RemoveValue(string name, string value)
        {
            this.attributes[name].RemoveValue(value);
        }

        public void RemoveValue(string name, UniqueIdentifier value)
        {
            this.attributes[name].RemoveValue(value);
        }

        public void RemoveValue(string name, Guid value)
        {
            this.attributes[name].RemoveValue(value);
        }

        public void SetValue(string name, byte[] value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, DateTime value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, bool value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, int value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, long value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, string value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, UniqueIdentifier value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, Guid value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, IEnumerable<byte[]> value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, IEnumerable<DateTime> value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, IEnumerable<bool> value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, IEnumerable<int> value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, IEnumerable<long> value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, IEnumerable<string> value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, IEnumerable<UniqueIdentifier> value)
        {
            this.attributes[name].SetValue(value);
        }

        public void SetValue(string name, IEnumerable<Guid> value)
        {
            this.attributes[name].SetValue(value);
        }

        internal void CompleteCreateOperation(UniqueIdentifier id)
        {
            if (this.ModificationType != OperationType.Create)
            {
                throw new InvalidOperationException();
            }

            AttributeTypeDefinition objectID = this.ObjectType["ObjectID"];
            if (!this.attributes.ContainsKey("ObjectID"))
            {
                this.attributes.Add("ObjectID", new AttributeValue(objectID, id));
            }
            else
            {
                this.attributes["ObjectID"] = new AttributeValue(objectID, id);
            }

            this.CommitChanges();

            this.ModificationType = OperationType.Update;
        }

        internal void CommitChanges()
        {
            foreach (var attribute in this.attributes.Values)
            {
                attribute.Commit();
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
                    string value;

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

                    PutFragmentType fragment = new PutFragmentType(kvp.Key, change.ChangeType, kvp.Key, null, false, value);
                    fragments.Add(fragment);
                }
            }

            return fragments;
        }

        public void Refresh()
        {
            // Re-gets the object from the FIM service
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Object Type: " + this.ObjectTypeName);

            foreach (AttributeValue value in this.attributes.Values)
            {
                sb.AppendLine(string.Format("{0}: {1}", value.AttributeName, value.ToString()));
            }

            return sb.ToString();
        }

        private void PopulateAttributeDefinitions()
        {
            foreach (AttributeTypeDefinition attributeDefinition in this.ObjectType)
            {
                if (!this.attributes.ContainsKey(attributeDefinition.SystemName))
                {
                    this.attributes.Add(attributeDefinition.SystemName, new AttributeValue(attributeDefinition));
                }
            }
        }

        public Dictionary<string, IList<string>> GetSerializationValues()
        {
            Dictionary<string, IList<string>> values = new Dictionary<string, IList<string>>();

            foreach (KeyValuePair<string, AttributeValue> kvp in this.attributes)
            {
                if (!kvp.Value.IsNull)
                {
                    values.Add(kvp.Key, kvp.Value.GetSerializationValues());
                }
            }

            return values;
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
