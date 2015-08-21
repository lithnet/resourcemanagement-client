using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.ResourceManagement.WebServices;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Contains the value or values of a specified attribute, and tracks changes made to the values
    /// </summary>
    public class AttributeValue
    {
        /// <summary>
        /// A static comparer object used to test for equality between values
        /// </summary>
        internal static AttributeValueEqualityComparer ValueComparer = new AttributeValueEqualityComparer();

        /// <summary>
        /// Gets the attribute that represents the values in this collection
        /// </summary>
        public AttributeTypeDefinition Attribute { get; private set; }

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
        /// An internal field used to track if the attribute value has been changed
        /// </summary>
        private bool hasChanged;

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
        /// Initializes a new instance of the AttributeValue class
        /// </summary>
        /// <param name="type">The definition of the attribute to hold the values for</param>
        internal AttributeValue(AttributeTypeDefinition type)
            : this(type, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AttributeValue class
        /// </summary>
        /// <param name="type">The definition of the attribute to hold the values for</param>
        /// <param name="value">The initial value of the attribute</param>
        internal AttributeValue(AttributeTypeDefinition type, object value)
        {
            this.Attribute = type;

            if (this.Attribute.IsMultivalued)
            {
                this.initialValues = new List<object>();
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
        /// Sets the internal initial value fields of the class to the current values
        /// </summary>
        internal void Commit()
        {
            if (this.Attribute.IsMultivalued)
            {
                this.initialValues.Clear();
                this.initialValues.AddRange(this.values);
            }
            else
            {
                this.initialValue = this.value;
            }

            this.hasChanged = false;
        }

        /// <summary>
        /// Reverts any changes made to the attribute values, restoring them to their initial state
        /// </summary>
        public void UndoChanges()
        {
            if (this.Attribute.IsMultivalued)
            {
                this.values.Clear();
                this.values.AddRange(this.initialValues);
            }
            else
            {
                this.value = this.initialValue;
            }

            this.hasChanged = false;
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

            this.hasChanged = !initialLoad;

            if (value == null)
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
                if (!this.values.Any(t => AttributeValue.ValueComparer.Equals(t, typedValue)))
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
                return this.values.Any(t => AttributeValue.ValueComparer.Equals(t, typedValue));
            }
            else
            {
                return AttributeValue.ValueComparer.Equals(this.value, typedValue);
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
            object typedValue = this.ConvertValueToAttributeType(value);

            if (this.Attribute.IsMultivalued)
            {
                this.values.RemoveAll(t => AttributeValue.ValueComparer.Equals(t, typedValue));
            }
            else
            {
                if (AttributeValue.ValueComparer.Equals(this.value, typedValue))
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

            this.hasChanged = !initialLoad;

            if (value == null)
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
                if (!AttributeValue.ValueComparer.Equals(this.value, this.initialValue))
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
                    foreach (object initialValue in this.initialValues)
                    {
                        tempValueChanges.Add(new AttributeValueChange(ModeType.Remove, initialValue));
                    }
                }
                else
                {
                    foreach (object newValue in this.values)
                    {
                        bool found = false;

                        foreach (object initialValue in this.initialValues)
                        {
                            if (AttributeValue.ValueComparer.Equals(initialValue, newValue))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            tempValueChanges.Add(new AttributeValueChange(ModeType.Insert, newValue));
                        }
                    }

                    foreach (object initialValue in this.initialValues)
                    {
                        bool found = false;

                        foreach (object newValue in this.values)
                        {
                            if (AttributeValue.ValueComparer.Equals(initialValue, newValue))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            tempValueChanges.Add(new AttributeValueChange(ModeType.Remove, initialValue));
                        }
                    }
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
            if (!((value is string) || (value is byte[]) || (value is int) || (value is long) || (value is bool) || (value is UniqueIdentifier) || (value is DateTime) || (value is ResourceObject)))
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
                        return (long)(int)value;
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
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.value == null ? 0 : this.value.GetHashCode();
        }

        /// <summary>
        /// Tests the current attribute value collection for equality with another object
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if the object has the same values as this object, or false if they are different</returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            else if (obj is AttributeValue)
            {
                AttributeValue obj2 = obj as AttributeValue;

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

                return AttributeValue.ValueComparer.Equals(obj2.Value, this.Value);

            }
            else
            {
                return AttributeValue.ValueComparer.Equals(obj, this.Value);
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

                return this.values.Cast<string>().ToList().AsReadOnly();
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

        /// <summary>
        /// Gets the values of the attribute converted to strings for serialization
        /// </summary>
        /// <returns></returns>
        internal IList<string> GetSerializationValues()
        {
            List<string> values = new List<string>();

            if (this.Attribute.IsMultivalued)
            {
                foreach (object value in this.values)
                {
                    values.Add(TypeConverter.ToString(value));
                }
            }
            else
            {
                values.Add(TypeConverter.ToString(this.value));
            }

            return values;
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
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
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
                return this.values.Select(t => TypeConverter.ToString(t)).ToCommaSeparatedString();
            }
            else
            {
                return TypeConverter.ToString(this.value);
            }
        }
    }
}


