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
    /// <summary>
    /// Represents a resource from the Resource Management Service
    /// </summary>
    [Serializable]
    [KnownType(typeof(List<string>))]
    public class ResourceObject : ISerializable
    {
        /// <summary>
        /// The client object used to pass save, update, and create operations to
        /// </summary>
        private ResourceManagementClient client;

        /// <summary>
        /// A list of attributes that should never be passed back to the Resource Management Service for updates
        /// </summary>
        private List<string> AttributesToIgnore;

        /// <summary>
        /// The internal representation of attributes of the object
        /// </summary>
        private AttributeValueCollection attributes;

        /// <summary>
        /// Gets the client object used for create, update, and delete operations of this object
        /// </summary>
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

        /// <summary>
        /// Gets the object type name of this object
        /// </summary>
        public string ObjectTypeName
        {
            get
            {
                return this.ObjectType.SystemName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this object is a placeholder, and has not been obtained directly from the Resource Management Service
        /// </summary>
        internal bool IsPlaceHolder { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this object has been deleted in the Resource Management Service
        /// </summary>
        internal bool IsDeleted { get; set; }

        /// <summary>
        /// Gets the type of modification that will be performed on this object the next time it is saved
        /// </summary>
        public OperationType ModificationType { get; private set; }

        /// <summary>
        /// Gets the object type definition for this object
        /// </summary>
        public ObjectTypeDefinition ObjectType { get; private set; }

        /// <summary>
        /// Gets the collection of attributes and values associated with this object
        /// </summary>
        public AttributeValueCollection Attributes
        {
            get
            {
                return this.attributes;
            }
        }

        /// <summary>
        /// Gets the object ID for this object
        /// </summary>
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

        /// <summary>
        /// Gets the display name of the object, or null if no display name is present
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the ResourceObject class
        /// </summary>
        /// <param name="opType">The type of modification to set on the object</param>
        /// <param name="client">The client used for further operations on this object</param>
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

        /// <summary>
        /// Initializes a new instance of the ResourceObject class
        /// </summary>
        /// <param name="info">The serialization information</param>
        /// <param name="context">The serialization context</param>
        protected ResourceObject(SerializationInfo info, StreamingContext context)
        {
            this.DeserializeObject(info);
        }

        /// <summary>
        /// Initializes a new instance of the ResourceObject class
        /// </summary>
        /// <param name="type">The object type that this object will represent</param>
        /// <param name="client">The client used for further operations on this object</param>
        internal ResourceObject(string type, ResourceManagementClient client)
            : this(OperationType.Create, client)
        {
            if (!ResourceManagementSchema.ObjectTypes.ContainsKey(type))
            {
                throw new ArgumentException(string.Format("Unknown object type {0}", type));
            }

            this.IsPlaceHolder = true;

            this.ObjectType = ResourceManagementSchema.ObjectTypes[type];
            this.AddRemainingAttributesFromSchema();

            this.attributes[AttributeNames.ObjectType].SetValue(type);
        }

        /// <summary>
        /// Initializes a new instance of the ResourceObject class
        /// </summary>
        /// <param name="type">The object type that this object will represent</param>
        /// <param name="id">The ID of the object</param>
        /// <param name="client">The client used for further operations on this object</param>
        internal ResourceObject(string type, UniqueIdentifier id, ResourceManagementClient client)
            : this(OperationType.Update, client)
        {
            if (!ResourceManagementSchema.ObjectTypes.ContainsKey(type))
            {
                throw new ArgumentException(string.Format("Unknown object type {0}", type));
            }

            this.ObjectType = ResourceManagementSchema.ObjectTypes[type];
            this.AddRemainingAttributesFromSchema();
            this.IsPlaceHolder = true;

            this.attributes[AttributeNames.ObjectType].SetValue(type);
            this.attributes[AttributeNames.ObjectID].SetValue(id);
        }

        /// <summary>
        /// Initializes a new instance of the ResourceObject class
        /// </summary>
        /// <param name="objectElements">An enumeration of XmlElements that make up a partial response</param>
        /// <param name="client">The client used for further operations on this object</param>
        internal ResourceObject(IEnumerable<XmlElement> objectElements, ResourceManagementClient client)
            : this(OperationType.Update, client)
        {
            this.PopulateResourceFromPartialResponse(objectElements);
        }

        /// <summary>
        /// Initializes a new instance of the ResourceObject class
        /// </summary>
        /// <param name="reader">An XmlDictionaryReader containing the full object definition</param>
        /// <param name="client">The client used for further operations on this object</param>
        internal ResourceObject(XmlDictionaryReader reader, ResourceManagementClient client)
            : this(OperationType.Update, client)
        {
            this.PopulateResourceFromFullObject(reader);
        }

        /// <summary>
        /// Initializes a new instance of the ResourceObject class
        /// </summary>
        /// <param name="element">An XmlElement containing definition of the object from a set of fragments obtained from an enumeration response</param>
        /// <param name="client">The client used for further operations on this object</param>
        internal ResourceObject(XmlElement element, ResourceManagementClient client)
            : this(OperationType.Update, client)
        {
            this.PopulateResourceFromFragment(element);
        }

        /// <summary>
        /// Gets the changes that are pending for this object
        /// </summary>
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

        /// <summary>
        /// Reverts the object back to its original state, undoing any pending attribute changes
        /// </summary>
        public void UndoChanges()
        {
            foreach (var attributeValue in this.attributes)
            {
                attributeValue.UndoChanges();
            }
        }

        /// <summary>
        /// Saves the changes to the Resource Management Service
        /// </summary>
        /// <param name="refresh">A value indicating if the object should be refreshed from the Resource Management Service after the changes have been made</param>
        public void Save(bool refresh)
        {
            this.Save();

            if (refresh)
            {
                this.Refresh();
            }
        }

        /// <summary>
        /// Saves the changes to the Resource Management Service
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether the specified attribute is present on the object and has value
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <returns>Returns true if the attribute has a value and false if the attribute is not present on the object or is null</returns>
        public bool HasValue(string name)
        {
            if (!this.attributes.ContainsAttribute(name))
            {
                return false;
            }

            return !attributes[name].IsNull;
        }

        /// <summary>
        /// Refreshes the object from the Resource Management Service
        /// </summary>
        /// <remarks>
        /// Note that this method will revert any pending changes on the object
        /// </remarks>
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

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string based list of attribute and value pairs</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (AttributeValue value in this.attributes)
            {
                sb.AppendLine(string.Format("{0}: {1}", value.AttributeName, TypeConverter.ToString(value)));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a list of attributes and values for the resource in a string format suitable for serialization
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, IList<string>> GetSerializationValues()
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

        /// <summary>
        /// Called by the parent ResourceManagementClient when a create operation against the Resource Management Service was successful
        /// </summary>
        /// <param name="id">The new object ID that was assigned this object</param>
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

        /// <summary>
        /// Calls the Commit() method on all the AttributeValue objects in the collection
        /// </summary>
        internal void CommitChanges()
        {
            foreach (AttributeValue attributeValues in this.attributes)
            {
                attributeValues.Commit();
            }
        }

        /// <summary>
        /// Gets a list of attribute changes as Put fragments for submission to the Resource Management Service
        /// </summary>
        /// <returns>A list of PutFragmentType objects containing all the value changes pending on the object</returns>
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

        /// <summary>
        /// Sets the internal attribute value collection with the initial values contained in the dictionary
        /// </summary>
        /// <param name="values">The initial attributes and values to set</param>
        private void SetInitialAttributeValues(Dictionary<string, List<string>> values)
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

                if (d != null)
                {
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
            }

            this.AddRemainingAttributesFromSchema();
        }

        /// <summary>
        /// Adds an empty AttributeValue object for the attributes present for this object class in the schema, but are not currently present on the object
        /// </summary>
        private void AddRemainingAttributesFromSchema()
        {
            foreach (AttributeTypeDefinition attributeDefinition in this.ObjectType.Attributes)
            {
                if (!this.attributes.ContainsAttribute(attributeDefinition.SystemName))
                {
                    this.attributes.Add(attributeDefinition.SystemName, new AttributeValue(attributeDefinition));
                }
            }
        }

        /// <summary>
        /// Populates the ResourceObject from its full object definition
        /// </summary>
        /// <param name="reader">The XmlDictionaryReader containing the full object definition</param>
        private void PopulateResourceFromFullObject(XmlDictionaryReader reader)
        {
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();

            reader.MoveToStartElement();

            string objectTypeName = reader.LocalName;
            this.ObjectType = ResourceManagementSchema.ObjectTypes[objectTypeName];

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

            this.SetInitialAttributeValues(values);
        }

        /// <summary>
        /// Populates the ResourceObject from a collection of fragments received from an enumeration response
        /// </summary>
        /// <param name="element">The parent XmlElement containing the object definition</param>
        private void PopulateResourceFromFragment(XmlElement element)
        {
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();

            string objectTypeName = element.LocalName;
            this.ObjectType = ResourceManagementSchema.ObjectTypes[objectTypeName];


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

            this.SetInitialAttributeValues(values);
        }

        /// <summary>
        /// Populates the ResourceObject from a partial response to a Get request
        /// </summary>
        /// <param name="objectElements">A collection of XmlElements defining the object</param>
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
                ObjectTypeDefinition objectType = ResourceManagementSchema.ObjectTypes[objectTypeName];
                this.ObjectType = objectType;
            }
            else
            {
                throw new ArgumentException("No object type was specified in the response");
            }

            this.SetInitialAttributeValues(values);
        }

        /// <summary>
        /// Gets the object data for serialization
        /// </summary>
        /// <param name="info">The serialization data</param>
        /// <param name="context">The serialization context</param>
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

            info.AddValue("_ModificationType", this.ModificationType.ToString());
        }

        /// <summary>
        /// Deserializes an object from a serialization data set
        /// </summary>
        /// <param name="info">The serialization data</param>
        private void DeserializeObject(SerializationInfo info)
        {
            this.AddRemainingAttributesFromSchema();
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();

            foreach (SerializationEntry entry in info)
            {
                if (entry.Name.StartsWith("_"))
                {
                    if (entry.Name == "_ModificationType")
                    {
                        this.ModificationType = (OperationType)Enum.Parse(typeof(OperationType), entry.Value.ToString());
                    }

                    continue;
                }

                IEnumerable<string> entryValues = entry.Value as IEnumerable<string>;

                if (entryValues != null)
                {
                    values.Add(entry.Name, entryValues.ToList());
                }
                else
                {
                    values.Add(entry.Name, new List<string>() { TypeConverter.ToString(entry.Value) });
                }
            }

            if (!values.ContainsKey(AttributeNames.ObjectType))
            {
                throw new InvalidOperationException("The object type of the attribute was not present in the serialization data");
            }

            if (this.ModificationType == OperationType.None)
            {
                this.ModificationType = OperationType.Update;
            }

            string objectTypeName = values[AttributeNames.ObjectType].First();
            this.ObjectType = ResourceManagementSchema.ObjectTypes[objectTypeName];
            this.SetInitialAttributeValues(values);
        }
    }
}