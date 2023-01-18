using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Contains the value or values of a specified attribute, and tracks changes made to the values
    /// </summary>
    public class AttributeValue : ISerializable
    {
        private bool hasPermissionHint;

        /// <summary>
        /// A static comparer object used to test for equality between values
        /// </summary>
        internal static AttributeValueEqualityComparer ValueComparer = new AttributeValueEqualityComparer();

        /// <summary>
        /// Gets the attribute that represents the values in this collection
        /// </summary>
        public AttributeTypeDefinition Attribute { get; private set; }

        /// <summary>
        /// Gets the permissions the user has on this attribute, if they are known. Permission hints can only be obtained with a Get request
        /// </summary>
        public AttributePermission PermissionHint { get; private set; }

        /// <summary>
        /// Gets a list of pending value changes for this attribute
        /// </summary>
        public ReadOnlyCollection<AttributeValueChange> ValueChanges
        {
            get
            {
                return new ReadOnlyCollection<AttributeValueChange>(this.GenerateValueChanges());
            }
        }

        /// <summary>
        /// The value of the attribute when the attribute is single-valued
        /// </summary>
        private object value;

        /// <summary>
        /// The initial value of the attribute when the attribute is single-valued
        /// </summary>
        private object initialValue;

        /// <summary>
        /// The values of the attribute when the attribute is multivalued
        /// </summary>
        private List<object> values;

        /// <summary>
        /// The initial values of the attribute when the attribute is multivalued
        /// </summary>
        private List<object> initialValues;

        /// <summary>
        /// A list of attribute values requested to be removed, but were not present on the attribute
        /// </summary>
        private List<object> blindRemovals;

        /// <summary>
        /// Gets the name of the attribute
        /// </summary>
        public string AttributeName
        {
            get
            {
                return this.Attribute.SystemName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value is null or empty
        /// </summary>
        public bool IsNull
        {
            get
            {
                if (this.Attribute.IsMultivalued)
                {
                    return this.values == null || this.values.Count == 0;
                }
                else
                {
                    return this.value == null;
                }
            }
        }

        /// <summary>
        /// This method has not been implemented
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization. </param>
        protected internal AttributeValue(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the AttributeValue class
        /// </summary>
        /// <param name="type">The definition of the attribute to hold the values for</param>
        internal AttributeValue(AttributeTypeDefinition type)
            : this(type, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AttributeValue class
        /// </summary>
        /// <param name="type">The definition of the attribute to hold the values for</param>
        /// <param name="value">The initial value of the attribute</param>
        internal AttributeValue(AttributeTypeDefinition type, object value)
            : this(type, null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AttributeValue class
        /// </summary>
        /// <param name="type">The definition of the attribute to hold the values for</param>
        /// <param name="permission">The user's permission on this attribute, if known</param>
        internal AttributeValue(AttributeTypeDefinition type, AttributePermission permission)
            : this(type, permission, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AttributeValue class
        /// </summary>
        /// <param name="type">The definition of the attribute to hold the values for</param>
        /// <param name="permission">The user's permission on this attribute, if known</param>
        /// <param name="value">The initial value of the attribute</param>
        internal AttributeValue(AttributeTypeDefinition type, AttributePermission? permission, object value)
        {
            this.Attribute = type;
            this.hasPermissionHint = permission != null;
            this.PermissionHint = permission ?? 0;

            if (this.Attribute.IsMultivalued)
            {
                this.initialValues = new List<object>();
                this.blindRemovals = new List<object>();
                this.values = new List<object>();
                this.SetMultiValue(value, true);
            }
            else
            {
                this.SetSingleValue(value, true);
            }
        }

        /// <summary>
        /// Gets or sets the raw value of the attribute
        /// </summary>
        public object Value
        {
            get
            {
                if (this.Attribute.IsMultivalued)
                {
                    return this.values;
                }
                else
                {
                    return this.value;
                }
            }
            set
            {
                this.SetValue(value);
            }
        }

        /// <summary>
        /// Gets the raw values of the attribute
        /// </summary>
        public ReadOnlyCollection<object> Values
        {
            get
            {
                if (this.Attribute.IsMultivalued)
                {
                    return this.values.AsReadOnly();
                }
                else
                {
                    return new List<object>() {this.value}.AsReadOnly();
                }
            }
        }

        /// <summary>
        /// Sets the internal initial value fields of the class to the current values
        /// </summary>
        internal void Commit()
        {
            if (this.Attribute.IsMultivalued)
            {
                this.initialValues.Clear();
                this.blindRemovals.Clear();
                this.initialValues.AddRange(this.values);
            }
            else
            {
                this.initialValue = this.value;
            }
        }

        /// <summary>
        /// Reverts any changes made to the attribute values, restoring them to their initial state
        /// </summary>
        public void UndoChanges()
        {
            if (this.Attribute.IsMultivalued)
            {
                this.values.Clear();
                this.blindRemovals.Clear();
                this.values.AddRange(this.initialValues);
            }
            else
            {
                this.value = this.initialValue;
            }
        }

        /// <summary>
        /// Sets the value of the attribute, overwriting any existing values present on the object
        /// </summary>
        /// <param name="value">The new value or values to set</param>
        public void SetValue(object value)
        {
            this.SetValue(value, false);
        }

        /// <summary>
        /// Sets the value of the attribute, overwriting any existing values present on the object
        /// </summary>
        /// <param name="value">The new value or values to set</param>
        /// <param name="initialLoad">A value indicating if the attribute is being set as part of an initial load, and represents the current value stored in the Resource Management Service</param>
        internal void SetValue(object value, bool initialLoad)
        {
            if (this.Attribute.IsMultivalued)
            {
                this.SetMultiValue(value, initialLoad);
            }
            else
            {
                this.SetSingleValue(value, initialLoad);
            }
        }

        /// <summary>
        /// Sets the value of the attribute when it is single-valued
        /// </summary>
        /// <param name="value">The value to set</param>
        /// <param name="initialLoad">A value indicating if the attribute value should be considered an initial value, rather than a change</param>
        private void SetSingleValue(object value, bool initialLoad)
        {
            if (!initialLoad && this.Attribute.IsReadOnly)
            {
                throw new ReadOnlyValueModificationException(this.Attribute);
            }

            if (value == null || value == DBNull.Value)
            {
                this.value = null;
                return;
            }
            else
            {
                this.value = this.ConvertValueToAttributeType(value);
            }

            if (initialLoad)
            {
                this.initialValue = this.value;
            }
        }

        /// <summary>
        /// Add the specified value to the collection of values of a multivalued attribute, or sets the value of a single-valued attribute
        /// </summary>
        /// <param name="value">The value to remove</param>
        public void AddValue(object value)
        {
            object typedValue = this.ConvertValueToAttributeType(value);

            if (this.Attribute.IsMultivalued)
            {
                if (!this.values.Any(t => ValueComparer.Equals(t, typedValue)))
                {
                    this.values.Add(typedValue);
                }

                //else
                //{
                //    throw new ArgumentException("The value provided already exists in the collection");
                //}
            }
            else
            {
                this.value = typedValue;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified value is present on this attribute
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <returns>True if the specified value is present, or false if it is not</returns>
        public bool HasValue(object value)
        {
            object typedValue = this.ConvertValueToAttributeType(value);

            if (this.Attribute.IsMultivalued)
            {
                return this.values.Any(t => ValueComparer.Equals(t, typedValue));
            }
            else
            {
                return ValueComparer.Equals(this.value, typedValue);
            }
        }

        /// <summary>
        /// Removes all values from the attribute
        /// </summary>
        public void RemoveValues()
        {
            if (this.Attribute.IsMultivalued)
            {
                this.values.Clear();
            }
            else
            {
                this.value = null;
            }
        }

        /// <summary>
        /// Removes a specific value from an attribute
        /// </summary>
        /// <param name="value">The value to remove</param>
        public void RemoveValue(object value)
        {
            this.RemoveValue(value, false);
        }

        /// <summary>
        /// Removes a specific value from an attribute
        /// </summary>
        /// <param name="value">The value to remove</param>
        /// <param name="removeIfNotPresent">A flag indicating if the client should attempt to delete the value from the FIM service, even if the value is not currently present in the local representation of the object</param>
        public void RemoveValue(object value, bool removeIfNotPresent)
        {
            object typedValue = this.ConvertValueToAttributeType(value);

            if (this.Attribute.IsMultivalued)
            {
                if (this.values.RemoveAll(t => ValueComparer.Equals(t, typedValue)) == 0)
                {
                    if (removeIfNotPresent)
                    {
                        this.blindRemovals.Add(typedValue);
                    }
                }
            }
            else
            {
                if (ValueComparer.Equals(this.value, typedValue))
                {
                    this.value = null;
                }
            }
        }

        /// <summary>
        /// Sets the value of an attribute when it is multivalued
        /// </summary>
        /// <param name="value">The value to set</param>
        /// <param name="initialLoad">A value indicating if the attribute value should be considered an initial value, rather than a change</param>
        private void SetMultiValue(object value, bool initialLoad)
        {
            if (!initialLoad && this.Attribute.IsReadOnly)
            {
                throw new ReadOnlyValueModificationException(this.Attribute);
            }

            if (value == null || value == DBNull.Value)
            {
                this.values.Clear();
                return;
            }

            IEnumerable collectionValues = value as IEnumerable;

            if (collectionValues == null || value is string || value is byte[])
            {
                this.values.Clear();

                // Only one value to set

                object convertedValue = this.ConvertValueToAttributeType(value);

                if (convertedValue != null)
                {
                    this.values.Add(convertedValue);

                    if (initialLoad)
                    {
                        this.initialValues.Add(convertedValue);
                    }
                }
            }
            else
            {
                this.values.Clear();

                foreach (object enumerableValue in collectionValues)
                {
                    if (enumerableValue == null)
                    {
                        throw new InvalidOperationException("A multivalued attribute cannot contain a null value");
                    }

                    object convertedValue = this.ConvertValueToAttributeType(enumerableValue);

                    if (convertedValue != null)
                    {
                        this.values.Add(convertedValue);

                        if (initialLoad)
                        {
                            this.initialValues.Add(convertedValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates a list of value changes based on the initial and current values of the attribute
        /// </summary>
        /// <returns>A list of changes</returns>
        private List<AttributeValueChange> GenerateValueChanges()
        {
            List<AttributeValueChange> tempValueChanges = new List<AttributeValueChange>();

            if (!this.Attribute.IsMultivalued)
            {
                if (!ValueComparer.Equals(this.value, this.initialValue))
                {
                    tempValueChanges.Add(new AttributeValueChange(ModeType.Modify, this.value));
                }
            }
            else
            {
                if (this.initialValues.Count == 0)
                {
                    foreach (object newValue in this.values)
                    {
                        tempValueChanges.Add(new AttributeValueChange(ModeType.Insert, newValue));
                    }
                }
                else if (this.values.Count == 0)
                {
                    foreach (object iv in this.initialValues)
                    {
                        tempValueChanges.Add(new AttributeValueChange(ModeType.Remove, iv));
                    }
                }
                else
                {
                    // Add values not found in the initial values
                    foreach (object newValue in this.values.Except(this.initialValues, ValueComparer))
                    {
                        tempValueChanges.Add(new AttributeValueChange(ModeType.Insert, newValue));
                    }

                    // Remove values that shouldn't exist anymore
                    foreach (object removedValue in this.initialValues.Except(this.values, ValueComparer))
                    {
                        tempValueChanges.Add(new AttributeValueChange(ModeType.Remove, removedValue));
                    }
                }

                foreach (object removedValue in this.blindRemovals)
                {
                    tempValueChanges.Add(new AttributeValueChange(ModeType.Remove, removedValue));
                }
            }

            return tempValueChanges;
        }

        /// <summary>
        /// Performs conversion of supported CLR types into the native data type format that matches the attribute definition
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>A value converted into the correct data type for the attribute</returns>
        private object ConvertValueToAttributeType(object value)
        {
            if (!(
                value is string ||
                value is byte[] ||
                value is int ||
                value is long ||
                value is bool ||
                value is UniqueIdentifier ||
                value is DateTime ||
                value is ResourceObject ||
                value is XPathExpression ||
                value is XPathDereferencedExpression))
            {
                throw new UnsupportedDataTypeException(this.Attribute.Type, value.GetType());
            }

            switch (this.Attribute.Type)
            {
                case AttributeType.Binary:
                    return TypeConverter.ToByte(value);

                case AttributeType.Boolean:
                    return TypeConverter.ToBoolean(value);

                case AttributeType.DateTime:
                    return TypeConverter.ToDateTime(value);

                case AttributeType.Integer:
                    if (value is int)
                    {
                        return (long) (int) value;
                    }
                    else
                    {
                        return TypeConverter.ToLong(value);
                    }

                case AttributeType.Reference:
                    return TypeConverter.ToUniqueIdentifier(value);

                case AttributeType.String:
                case AttributeType.Text:
                    return TypeConverter.ToString(value);

                default:
                case AttributeType.Unknown:
                    throw new InvalidOperationException("The attribute type was unknown");
            }
        }

        /// <summary>
        /// Gets the hash code for this object
        /// </summary>
        /// <returns>The hash code for the object</returns>
        public override int GetHashCode()
        {
            return this.value?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Tests the current attribute value collection for equality with another object
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if the object has the same values as this object, or false if they are different</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            AttributeValue obj2 = obj as AttributeValue;

            if (obj2 != null)
            {
                if (obj2.AttributeName != this.AttributeName)
                {
                    return false;
                }

                if (obj2.IsNull && this.IsNull)
                {
                    return true;
                }

                if (obj2.IsNull || this.IsNull)
                {
                    return false;
                }

                return ValueComparer.Equals(obj2.Value, this.Value);
            }
            else
            {
                return ValueComparer.Equals(obj, this.Value);
            }
        }

        /// <summary>
        /// Gets the value of a single-valued attribute as a string
        /// </summary>
        public string StringValue
        {
            get
            {
                if (this.Attribute.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a multivalued attribute value from GetValue");
                }

                return TypeConverter.ToString(this.value);
            }
        }

        /// <summary>
        /// Gets the value of a single-valued attribute as a 64-bit integer
        /// </summary>
        public long IntegerValue
        {
            get
            {
                if (this.Attribute.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a multivalued attribute value from GetValue");
                }

                return TypeConverter.ToLong(this.value);
            }
        }

        /// <summary>
        /// Gets the value of a single-valued attribute as a boolean value
        /// </summary>
        public bool BooleanValue
        {
            get
            {
                if (this.Attribute.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a multivalued attribute value from GetValue");
                }

                return TypeConverter.ToBoolean(this.value);
            }
        }

        /// <summary>
        /// Gets the value of a single-valued attribute as a byte array
        /// </summary>
        public byte[] BinaryValue
        {
            get
            {
                if (this.Attribute.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a multivalued attribute value from GetValue");
                }

                if (this.IsNull)
                {
                    return null;
                }

                return TypeConverter.ToByte(this.value);
            }
        }

        /// <summary>
        /// Gets the value of a single-valued attribute as a reference value
        /// </summary>
        public UniqueIdentifier ReferenceValue
        {
            get
            {
                if (this.Attribute.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a multivalued attribute value from GetValue");
                }

                return TypeConverter.ToUniqueIdentifier(this.value);
            }
        }

        /// <summary>
        /// Gets the value of a single-valued attribute as a DateTime value
        /// </summary>
        public DateTime DateTimeValue
        {
            get
            {
                if (this.Attribute.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a multivalued attribute value from GetValue");
                }

                return TypeConverter.ToDateTime(this.value);
            }
        }

        /// <summary>
        /// Gets the values of a multivalued attribute as a collection of string values
        /// </summary>
        public ReadOnlyCollection<string> StringValues
        {
            get
            {
                if (!this.Attribute.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a single-valued attribute value from GetValues");
                }

                return this.values.ConvertAll(TypeConverter.ToString).AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the values of the attribute as a collection of string values
        /// </summary>
        public ReadOnlyCollection<string> ValuesAsString
        {
            get
            {
                if (this.Attribute.IsMultivalued)
                {
                    return this.values.ConvertAll(TypeConverter.ToString).AsReadOnly();
                }
                else
                {
                    return new List<string>() {TypeConverter.ToString(this.value)}.AsReadOnly();
                }
            }
        }

        /// <summary>
        /// Gets the values of a multivalued attribute as a collection of 64-bit integer values
        /// </summary>
        public ReadOnlyCollection<long> IntegerValues
        {
            get
            {
                if (!this.Attribute.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a single-valued attribute value from GetValues");
                }

                return this.values.Cast<long>().ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the values of a multivalued attribute as a collection of byte array values
        /// </summary>
        public ReadOnlyCollection<byte[]> BinaryValues
        {
            get
            {
                if (!this.Attribute.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a single-valued attribute value from GetValues");
                }

                return this.values.Cast<byte[]>().ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the values of a multivalued attribute as a collection of reference values
        /// </summary>
        public ReadOnlyCollection<UniqueIdentifier> ReferenceValues
        {
            get
            {
                if (!this.Attribute.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a single-valued attribute value from GetValues");
                }

                return this.values.Cast<UniqueIdentifier>().ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the values of a multivalued attribute as a collection of DateTime values
        /// </summary>
        public ReadOnlyCollection<DateTime> DateTimeValues
        {
            get
            {
                if (!this.Attribute.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a single-valued attribute value from GetValues");
                }

                return this.values.Cast<DateTime>().ToList().AsReadOnly();
            }
        }

        /// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization. </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ResourceSerializationSettings settings = context.Context as ResourceSerializationSettings? ?? new ResourceSerializationSettings();

            this.Serialize(info, settings);
        }

        internal void Serialize(SerializationInfo info, ResourceSerializationSettings settings)
        {
            if (settings.ResourceFormat == ResourceSerializationHandling.FixedStructure)
            {
                info.AddValue("Name", this.AttributeName);
            }

            if (this.hasPermissionHint && settings.IncludePermissionHints)
            {
                List<string> permissions = this.PermissionHint.ToList();

                if (permissions.Count > 0)
                {
                    info.AddValue("PermissionHint", permissions);
                }
                else
                {
                    info.AddValue("PermissionHint", null);
                }
            }

            this.SerializeValues(info, settings, "Values");
        }

        internal void SerializeValues(SerializationInfo info, ResourceSerializationSettings settings, string elementName)
        {
            if (this.IsNull)
            {
                if (settings.IncludeNullValues)
                {
                    if (settings.ArrayHandling == ArraySerializationHandling.AllAttributes ||
                        settings.ResourceFormat == ResourceSerializationHandling.FixedStructure ||
                        (this.Attribute.IsMultivalued && settings.ArrayHandling != ArraySerializationHandling.WhenRequired))
                    {
                        info.AddValue(elementName, new object[0]);
                    }
                    else
                    {
                        info.AddValue(elementName, null);
                    }
                }

                return;
            }

            IList<object> serializedValues = this.GetSerializationValues(settings);

            if (this.Attribute.IsMultivalued)
            {
                if (serializedValues.Count == 1 && settings.ArrayHandling == ArraySerializationHandling.WhenRequired && settings.ResourceFormat != ResourceSerializationHandling.FixedStructure)
                {
                    info.AddValue(elementName, serializedValues.First());
                }
                else
                {
                    info.AddValue(elementName, serializedValues);
                }
            }
            else
            {
                if (settings.ArrayHandling == ArraySerializationHandling.AllAttributes ||
                    settings.ResourceFormat == ResourceSerializationHandling.FixedStructure)
                {
                    info.AddValue(elementName, serializedValues);
                }
                else
                {
                    info.AddValue(elementName, serializedValues.First());
                }
            }
        }

        /// <summary>
        /// Gets the values of the attribute converted to types suitable for serialization
        /// </summary>
        /// <returns></returns>
        internal IList<object> GetSerializationValues()
        {
            return this.GetSerializationValues(new ResourceSerializationSettings());
        }

        /// <summary>
        /// Gets the values of the attribute converted to types suitable for serialization
        /// </summary>
        /// <param name="settings">The settings used to control serialization</param>
        /// <returns></returns>
        internal IList<object> GetSerializationValues(ResourceSerializationSettings settings)
        {
            List<object> serializationValues = new List<object>();

            if (this.Attribute.IsMultivalued)
            {
                foreach (object v in this.values)
                {
                    if (settings.ValueFormat == AttributeValueSerializationHandling.ConvertToString)
                    {
                        serializationValues.Add(TypeConverter.ToString(v));
                    }
                    else
                    {
                        serializationValues.Add(TypeConverter.ToSerializableValue(v));
                    }
                }
            }
            else
            {
                if (settings.ValueFormat == AttributeValueSerializationHandling.ConvertToString)
                {
                    serializationValues.Add(TypeConverter.ToString(this.value));
                }
                else
                {
                    serializationValues.Add(TypeConverter.ToSerializableValue(this.value));
                }
            }

            return serializationValues;
        }

        /// <summary>
        /// Compares the values within an AttributeValue object for equality with another object
        /// </summary>
        /// <param name="a">The AttributeValue object containing the values to compare</param>
        /// <param name="b">An object to compare</param>
        /// <returns>True if the value or values of the AttributeValue object match the value or values provided</returns>
        public static bool operator ==(AttributeValue a, object b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        /// <summary>
        /// Compares the values within an AttributeValue object for non-equality with another object
        /// </summary>
        /// <param name="a">The AttributeValue object containing the values to compare</param>
        /// <param name="b">An object to compare</param>
        /// <returns>True if the value or values of the AttributeValue object do not match the value or values provided</returns>
        public static bool operator !=(AttributeValue a, object b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns the values of this object as a comma separated string
        /// </summary>
        /// <returns>A comma separated string</returns>
        public override string ToString()
        {
            if (this.Attribute.IsMultivalued)
            {
                return this.values.Select(TypeConverter.ToString).ToCommaSeparatedString();
            }
            else
            {
                return TypeConverter.ToString(this.value);
            }
        }

        /// <summary>
        /// Returns the values of this object as a string enumeration
        /// </summary>
        /// <returns>A comma separated string</returns>
        public IEnumerable<string> ToStringValues()
        {
            if (this.Attribute.IsMultivalued)
            {
                foreach (object v in this.values)
                {
                    yield return TypeConverter.ToString(v);
                }
            }
            else
            {
                if (this.value != null)
                {
                    yield return TypeConverter.ToString(this.value);
                }
            }
        }
    }
}