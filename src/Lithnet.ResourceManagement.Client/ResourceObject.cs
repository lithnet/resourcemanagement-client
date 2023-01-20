﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Nito.AsyncEx;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Represents a resource from the Resource Management Service
    /// </summary>
    [Serializable]
    [KnownType(typeof(List<string>))]
    [KnownType(typeof(List<object>))]
    public class ResourceObject : ISerializable
    {
        private IClientFactory clientFactory;

        /// <summary>
        /// Gets the object type name of this object
        /// </summary>
        public string ObjectTypeName => this.ObjectType.SystemName;

        /// <summary>
        /// Gets a value indicating whether this object is a placeholder, and has not been obtained directly from the Resource Management Service
        /// </summary>
        internal bool IsPlaceHolder
        {
            get; private set;
        }

        /// <summary>
        /// Gets a value indicating whether this object has been deleted in the Resource Management Service
        /// </summary>
        internal bool IsDeleted
        {
            get; set;
        }

        /// <summary>
        /// Gets the type of modification that will be performed on this object the next time it is saved
        /// </summary>
        public OperationType ModificationType
        {
            get; private set;
        }

        /// <summary>
        /// Gets the object type definition for this object
        /// </summary>
        public ObjectTypeDefinition ObjectType
        {
            get; private set;
        }

        /// <summary>
        /// Gets the localization culture of this object
        /// </summary>
        public CultureInfo Locale
        {
            get; private set;
        }

        /// <summary>
        /// Gets the collection of attributes and values associated with this object
        /// </summary>
        public AttributeValueCollection Attributes
        {
            get; private set;
        }

        /// <summary>
        /// Gets a value indicating if this object has attribute permission hints available
        /// </summary>
        public bool HasPermissionHints
        {
            get; private set;
        }

        /// <summary>
        /// Gets a value indicating if this object is in a read only state
        /// </summary>
        public bool IsReadOnly => this.clientFactory == null;

        /// <summary>
        /// Gets the object ID for this object
        /// </summary>
        public UniqueIdentifier ObjectID
        {
            get
            {
                if (this.Attributes.ContainsAttribute(AttributeNames.ObjectID) && this.Attributes[AttributeNames.ObjectID].ReferenceValue != null)
                {
                    return this.Attributes[AttributeNames.ObjectID].ReferenceValue;
                }
                else
                {
                    // Generate and store new GUID on object
                    UniqueIdentifier newId = new UniqueIdentifier(Guid.NewGuid());
                    AttributeTypeDefinition objectID = this.ObjectType[AttributeNames.ObjectID];
                    if (!this.Attributes.ContainsAttribute(AttributeNames.ObjectID))
                    {
                        this.Attributes.Add(new AttributeValue(objectID, newId));
                    }
                    else
                    {
                        this.Attributes[AttributeNames.ObjectID] = new AttributeValue(objectID, newId);
                    }

                    return newId;
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
                if (this.Attributes.ContainsAttribute(AttributeNames.DisplayName))
                {
                    return this.Attributes[AttributeNames.DisplayName].StringValue;
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
        /// <param name="clientFactory">The client used for further operations on this object</param>
        /// <param name="locale">The localization culture that this object is represented as</param>
        private ResourceObject(OperationType opType, IClientFactory clientFactory, CultureInfo locale)
        {
            this.ModificationType = opType;
            this.Attributes = new AttributeValueCollection();
            this.clientFactory = clientFactory;
            this.Locale = locale;
        }

        /// <summary>
        /// Initializes a new instance of the ResourceObject class
        /// </summary>
        /// <param name="info">The serialization information</param>
        /// <param name="context">The serialization context</param>
        protected ResourceObject(SerializationInfo info, StreamingContext context)
        {
            this.DeserializeObject(info, context);
        }

        /// <summary>
        /// Initializes a new instance of the ResourceObject class
        /// </summary>
        /// <param name="type">The object type that this object will represent</param>
        /// <param name="clientFactory">The client used for further operations on this object</param>
        internal ResourceObject(string type, IClientFactory clientFactory)
            : this(OperationType.Create, clientFactory, null)
        {
            this.IsPlaceHolder = true;
            this.SetObjectType(type);
            this.AddRemainingAttributesFromSchema();
            this.Attributes[AttributeNames.ObjectType].SetValue(type);
        }

        /// <summary>
        /// Initializes a new instance of the ResourceObject class
        /// </summary>
        /// <param name="type">The object type that this object will represent</param>
        /// <param name="id">The ID of the object</param>
        /// <param name="clientFactory">The client used for further operations on this object</param>
        internal ResourceObject(string type, UniqueIdentifier id, IClientFactory clientFactory)
            : this(OperationType.Update, clientFactory, null)
        {
            this.SetObjectType(type);
            this.AddRemainingAttributesFromSchema();
            this.IsPlaceHolder = true;

            this.Attributes[AttributeNames.ObjectType].SetValue(type, true);
            this.Attributes[AttributeNames.ObjectID].SetValue(id, true);
        }

        /// <summary>
        /// Initializes a new instance of the ResourceObject class
        /// </summary>
        /// <param name="objectElements">An enumeration of XmlElements that make up a partial response</param>
        /// <param name="clientFactory">The client used for further operations on this object</param>
        /// <param name="locale">The localization culture that this object is represented as</param>
        internal ResourceObject(IEnumerable<XmlElement> objectElements, IClientFactory clientFactory, CultureInfo locale) : this(OperationType.Update, clientFactory, locale)
        {
            this.PopulateResourceFromPartialResponse(objectElements);
        }

        /// <summary>
        /// Initializes a new instance of the ResourceObject class
        /// </summary>
        /// <param name="reader">An XmlDictionaryReader containing the full object definition</param>
        /// <param name="clientFactory">The client used for further operations on this object</param>
        /// <param name="locale">The localization culture that this object is represented as</param>
        internal ResourceObject(XmlDictionaryReader reader, IClientFactory clientFactory, CultureInfo locale) : this(OperationType.Update, clientFactory, locale)
        {
            this.PopulateResourceFromFullObject(reader);
        }

        /// <summary>
        /// Initializes a new instance of the ResourceObject class
        /// </summary>
        /// <param name="element">An XmlElement containing definition of the object from a set of fragments obtained from an enumeration response</param>
        /// <param name="clientFactory">The client used for further operations on this object</param>
        /// <param name="locale">The localization culture that this object is represented as</param>
        internal ResourceObject(XmlElement element, IClientFactory clientFactory, CultureInfo locale)
            : this(OperationType.Update, clientFactory, locale)
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

                foreach (AttributeValue attributeValue in this.Attributes)
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
            foreach (var attributeValue in this.Attributes)
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
            AsyncContext.Run(async () => await this.SaveAsync(refresh).ConfigureAwait(false));
        }

        /// <summary>
        /// Saves the changes to the Resource Management Service
        /// </summary>
        /// <param name="refresh">A value indicating if the object should be refreshed from the Resource Management Service after the changes have been made</param>
        public async Task SaveAsync(bool refresh)
        {
            await this.SaveAsync().ConfigureAwait(false);

            if (refresh)
            {
                await this.RefreshAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Saves the changes to the Resource Management Service
        /// </summary>
        public void Save()
        {
            AsyncContext.Run(async () => await this.SaveAsync().ConfigureAwait(false));
        }

        /// <summary>
        /// Saves the changes to the Resource Management Service
        /// </summary>
        public async Task SaveAsync()
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("The object is a read-only copy");
            }

            switch (this.ModificationType)
            {
                case OperationType.Create:
                    await this.clientFactory.ResourceFactoryClient.CreateAsync(this).ConfigureAwait(false);
                    break;

                case OperationType.Update:
                    await this.clientFactory.ResourceClient.PutAsync(this, this.Locale).ConfigureAwait(false);
                    break;

                case OperationType.Delete:
                    if (this.IsDeleted)
                    {
                        throw new InvalidOperationException("The object has already been deleted");
                    }

                    await this.clientFactory.ResourceClient.DeleteAsync(this).ConfigureAwait(false);
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
            if (!this.Attributes.ContainsAttribute(name))
            {
                return false;
            }

            return !this.Attributes[name].IsNull;
        }

        /// <summary>
        /// Refreshes the object from the Resource Management Service
        /// </summary>
        /// <remarks>
        /// Note that this method will revert any pending changes on the object
        /// </remarks>
        public void Refresh()
        {
            AsyncContext.Run(async () => await this.RefreshAsync().ConfigureAwait(false));
        }

        /// <summary>
        /// Refreshes the object from the Resource Management Service
        /// </summary>
        /// <remarks>
        /// Note that this method will revert any pending changes on the object
        /// </remarks>
        public async Task RefreshAsync()
        {
            if (this.IsPlaceHolder)
            {
                throw new InvalidOperationException("Cannot refresh a placeholder object. Call Save() to submit the changes to the resource management service");
            }

            if (this.IsDeleted)
            {
                throw new InvalidOperationException("Cannot refresh the object as it has been deleted");
            }

            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("The object is a read-only copy");
            }

            XmlDictionaryReader reader = await this.clientFactory.ResourceClient.GetFullObjectForUpdateAsync(this).ConfigureAwait(false);

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

            foreach (KeyValuePair<string, IList<object>> kvp in this.GetSerializationValues())
            {
                foreach (string value in kvp.Value)
                {
                    sb.AppendLine(string.Format("{0}: {1}", kvp.Key, value));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a list of attributes and values for the resource in a primative format suitable for serialization
        /// </summary>
        /// <returns>A list of attribute and value pairs</returns>
        internal Dictionary<string, IList<object>> GetSerializationValues()
        {
            return this.GetSerializationValues(new ResourceSerializationSettings());
        }

        /// <summary>
        /// Gets a list of attributes and values for the resource in a primative format suitable for serialization
        /// </summary>
        /// <param name="settings">The settings used to serialize the resource</param>
        /// <returns>A list of attribute and value pairs</returns>
        internal Dictionary<string, IList<object>> GetSerializationValues(ResourceSerializationSettings settings)
        {
            Dictionary<string, IList<object>> values = new Dictionary<string, IList<object>>();

            foreach (AttributeValue kvp in this.Attributes)
            {
                if (!kvp.IsNull)
                {
                    values.Add(kvp.AttributeName, kvp.GetSerializationValues(settings));
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
            if (!this.Attributes.ContainsAttribute(AttributeNames.ObjectID))
            {
                this.Attributes.Add(new AttributeValue(objectID, id));
            }
            else
            {
                this.Attributes[AttributeNames.ObjectID] = new AttributeValue(objectID, id);
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
            foreach (AttributeValue attributeValues in this.Attributes)
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

                if (SchemaConstants.ComputedAttributes.Contains(type.SystemName))
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
        /// <param name="permissions">The permission hints for the attributes</param>
        private void SetInitialAttributeValues(Dictionary<string, List<string>> values, Dictionary<string, AttributePermission> permissions)
        {
            this.Attributes = new AttributeValueCollection();
            this.HasPermissionHints = false;

            foreach (KeyValuePair<string, List<string>> kvp in values)
            {
                string attributeName = kvp.Key;
                AttributeTypeDefinition d = this.ObjectType[attributeName];
                AttributePermission p = AttributePermission.Unknown;

                if (permissions != null)
                {
                    if (permissions.ContainsKey(attributeName))
                    {
                        p = permissions[attributeName];

                        if (p != AttributePermission.Unknown)
                        {
                            this.HasPermissionHints = true;
                        }
                    }
                }

                if (d != null)
                {
                    if (!d.IsMultivalued && kvp.Value.Count > 1)
                    {
                        throw new InvalidOperationException($"The attribute {d.SystemName} is listed in the schema as a multivalued attribute, but more than one value was returned");
                    }

                    if (kvp.Value.Count == 0)
                    {
                        this.Attributes.Add(d.SystemName, new AttributeValue(d, p));
                        continue;
                    }

                    if (d.IsMultivalued)
                    {
                        this.Attributes.Add(d.SystemName, new AttributeValue(d, p, kvp.Value));
                    }
                    else
                    {
                        this.Attributes.Add(d.SystemName, new AttributeValue(d, p, kvp.Value.First()));
                    }

                    if (d.SystemName == AttributeNames.Locale)
                    {
                        this.Locale = new CultureInfo(kvp.Value.First());
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
                if (!this.Attributes.ContainsAttribute(attributeDefinition.SystemName))
                {
                    this.Attributes.Add(attributeDefinition.SystemName, new AttributeValue(attributeDefinition));
                }
            }
        }

        /// <summary>
        /// Populates the ResourceObject from its full object definition received from a Get request
        /// </summary>
        /// <param name="reader">The XmlDictionaryReader containing the full object definition</param>
        private void PopulateResourceFromFullObject(XmlDictionaryReader reader)
        {
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();
            Dictionary<string, AttributePermission> permissions = new Dictionary<string, AttributePermission>();

            reader.MoveToStartElement();

            string objectTypeName = reader.LocalName;
            this.SetObjectType(objectTypeName);

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

                AttributePermission p;
                if (Enum.TryParse(reader.GetAttribute("permissions", Namespaces.ResourceManagement), out p))
                {
                    if (!permissions.ContainsKey(reader.LocalName))
                    {
                        permissions.Add(reader.LocalName, p);
                    }
                    else
                    {
                        permissions[reader.LocalName] = p;
                    }
                }

                string value = reader.ReadString();
                if (!string.IsNullOrEmpty(value))
                {
                    values[reader.LocalName].Add(value);
                }
            }

            this.SetInitialAttributeValues(values, permissions);
        }

        /// <summary>
        /// Populates the ResourceObject from a collection of fragments received from an enumeration response
        /// </summary>
        /// <param name="element">The parent XmlElement containing the object definition</param>
        private void PopulateResourceFromFragment(XmlElement element)
        {
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();

            string objectTypeName = element.LocalName;
            this.SetObjectType(objectTypeName);

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

            this.SetInitialAttributeValues(values, null);
        }

        /// <summary>
        /// Populates the ResourceObject from a partial response to a Get request
        /// </summary>
        /// <param name="objectElements">A collection of XmlElements defining the object</param>
        private void PopulateResourceFromPartialResponse(IEnumerable<XmlElement> objectElements)
        {
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();
            Dictionary<string, AttributePermission> permissions = new Dictionary<string, AttributePermission>();

            foreach (XmlElement partialAttributeElement in objectElements.Where(t => t.LocalName == "PartialAttribute"))
            {
                foreach (XmlElement attributeElement in partialAttributeElement.ChildNodes.OfType<XmlElement>())
                {
                    if (!values.ContainsKey(attributeElement.LocalName))
                    {
                        values.Add(attributeElement.LocalName, new List<string>());
                    }

                    AttributePermission p;
                    if (Enum.TryParse(attributeElement.GetAttribute("permissions", Namespaces.ResourceManagement), out p))
                    {
                        if (!permissions.ContainsKey(attributeElement.LocalName))
                        {
                            permissions.Add(attributeElement.LocalName, p);
                        }
                        else
                        {
                            permissions[attributeElement.LocalName] = p;
                        }
                    }

                    string value = attributeElement.InnerText;

                    if (!string.IsNullOrEmpty(value))
                    {
                        values[attributeElement.LocalName].Add(value);
                    }
                }
            }

            if (values.ContainsKey(AttributeNames.ObjectType))
            {
                string objectTypeName = values[AttributeNames.ObjectType].First();
                this.SetObjectType(objectTypeName);
            }
            else
            {
                throw new ArgumentException("No object type was specified in the response");
            }

            this.SetInitialAttributeValues(values, permissions);
        }

        /// <summary>
        /// Gets the object data for serialization
        /// </summary>
        /// <param name="info">The serialization data</param>
        /// <param name="context">The serialization context</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ResourceSerializationSettings settings = context.Context as ResourceSerializationSettings? ?? new ResourceSerializationSettings();

            if (settings.ResourceFormat == ResourceSerializationHandling.FixedStructure || settings.IncludePermissionHints)
            {
                this.SerializeObjectComplex(info, settings);
            }
            else
            {
                this.SerializeObjectSimple(info, settings);
            }
        }

        private void SerializeObjectComplex(SerializationInfo info, ResourceSerializationSettings settings)
        {
            if (settings.ResourceFormat == ResourceSerializationHandling.FixedStructure)
            {
                info.AddValue("Resource", this.Attributes.Where(t => !t.IsNull || settings.IncludeNullValues));
            }
            else
            {
                foreach (AttributeValue a in this.Attributes)
                {
                    if (a.IsNull && !settings.IncludeNullValues)
                    {
                        continue;
                    }

                    info.AddValue(a.AttributeName, a);
                }
            }
        }

        private void SerializeObjectSimple(SerializationInfo info, ResourceSerializationSettings settings)
        {
            foreach (AttributeValue a in this.Attributes)
            {
                a.SerializeValues(info, settings, a.AttributeName);
            }
        }

        /// <summary>
        /// Deserializes an object from a serialization data set
        /// </summary>
        /// <param name="info">The serialization data</param>
        private void DeserializeObject(SerializationInfo info, StreamingContext context)
        {
            if (context.Context is ResourceManagementClient c)
            {
                this.clientFactory = c.ClientFactory;
            }

            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();

            foreach (SerializationEntry entry in info)
            {
                if (entry.Value is IEnumerable<string> entryValues)
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
            this.SetObjectType(objectTypeName);
            this.SetInitialAttributeValues(values, null);
            this.AddRemainingAttributesFromSchema();
        }

        private void SetObjectType(string type)
        {
            this.ObjectType = AsyncContext.Run(async () => await this.clientFactory.SchemaClient.GetObjectTypeAsync(type).ConfigureAwait(false));
        }

    }
}