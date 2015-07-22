using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.ResourceManagement.WebServices;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;

namespace Lithnet.ResourceManagement.Client
{
    public class AttributeValue
    {
        internal static AttributeValueEqualityComparer ValueComparer = new AttributeValueEqualityComparer();

        public AttributeTypeDefinition Attribute { get; private set; }

        public ReadOnlyCollection<AttributeValueChange> ValueChanges
        {
            get
            {
                return new ReadOnlyCollection<AttributeValueChange>(this.GenerateValueChanges());
            }
        }

        private object value;

        private object initialValue;

        private List<object> values;

        private List<object> initialValues;

        private bool hasChanged;

        public bool IsReadOnly
        {
            get
            {
                return this.Attribute.IsReadOnly;
            }
        }

        public bool IsRequired
        {
            get
            {
                return this.Attribute.IsRequired;
            }
        }

        public bool IsMultivalued
        {
            get
            {
                return this.Attribute.IsMultivalued;
            }
        }

        public string AttributeName
        {
            get
            {
                return this.Attribute.SystemName;
            }
        }

        public bool IsNull
        {
            get
            {
                if (this.IsMultivalued)
                {
                    return this.values == null || this.values.Count == 0;
                }
                else
                {
                    return this.value == null;
                }
            }
        }

        internal AttributeValue(AttributeTypeDefinition type)
            : this(type, null)
        {
        }

        internal AttributeValue(AttributeTypeDefinition type, object value)
        {
            this.Attribute = type;

            if (this.IsMultivalued)
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

        public object Value
        {
            get
            {
                if (this.IsMultivalued)
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

        internal void Commit()
        {
            if (this.IsMultivalued)
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

        public void UndoChanges()
        {
            if (this.IsMultivalued)
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

        public void SetValue(object value)
        {
            if (this.IsMultivalued)
            {
                this.SetMultiValue(value, false);
            }
            else
            {
                this.SetSingleValue(value, false);
            }
        }

        private void SetSingleValue(object value, bool initialLoad)
        {
            if (!initialLoad && this.IsReadOnly)
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

        public void AddValue(object value)
        {
            object typedValue = this.ConvertValueToAttributeType(value);

            if (this.IsMultivalued)
            {
                if (!this.values.Any(t => AttributeValue.ValueComparer.Equals(t, typedValue)))
                {
                    this.values.Add(typedValue);
                }
                else
                {
                    throw new ArgumentException("The value provided already exists in the collection");
                }
            }
            else
            {
                this.value = typedValue;
            }
        }

        public void RemoveValues()
        {
            if (this.IsMultivalued)
            {
                this.values.Clear();
            }
            else
            {
                this.value = null;
            }
        }

        public void RemoveValue(object value)
        {
            object typedValue = this.ConvertValueToAttributeType(value);

            if (this.IsMultivalued)
            {
                this.values.RemoveAll(t => AttributeValue.ValueComparer.Equals(t, value));
            }
            else
            {
                if (AttributeValue.ValueComparer.Equals(this.value, value))
                {
                    this.value = null;
                }
            }
        }

        private void SetMultiValue(object value, bool initialLoad)
        {
            if (!initialLoad && this.IsReadOnly)
            {
                throw new ReadOnlyValueModificationException(this.Attribute);
            }

            this.hasChanged = !initialLoad;

            if (value == null)
            {
                this.values.Clear();
            }
            else
            {
                IEnumerable collectionValues = value as IEnumerable;

                if (collectionValues == null || value is string || value is byte[])
                {
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
        }

        private List<AttributeValueChange> GenerateValueChanges()
        {
            List<AttributeValueChange> tempValueChanges = new List<AttributeValueChange>();

            if (!this.IsMultivalued)
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

        private object ConvertValueToAttributeType(object value)
        {
            if (!((value is string) || (value is byte[]) || (value is int) || (value is long) || (value is bool) || (value is UniqueIdentifier) || (value is DateTime)))
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

        public override int GetHashCode()
        {
            return this.value == null ? 0 : this.value.GetHashCode();
        }

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

        public string StringValue
        {
            get
            {
                if (this.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a multivalued attribute value from GetValue");
                }

                return TypeConverter.ToString(this.value);
            }
        }

        public long IntegerValue
        {
            get
            {
                if (this.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a multivalued attribute value from GetValue");
                }

                return TypeConverter.ToLong(this.value);
            }
        }

        public bool BooleanValue
        {
            get
            {
                if (this.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a multivalued attribute value from GetValue");
                }

                return TypeConverter.ToBoolean(this.value);
            }
        }

        public byte[] BinaryValue
        {
            get
            {
                if (this.IsMultivalued)
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

        public UniqueIdentifier ReferenceValue
        {
            get
            {
                if (this.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a multivalued attribute value from GetValue");
                }

                return TypeConverter.ToUniqueIdentifier(this.value);
            }
        }

        public DateTime DateTimeValue
        {
            get
            {
                if (this.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a multivalued attribute value from GetValue");
                }

                return TypeConverter.ToDateTime(this.value);
            }
        }

        public ReadOnlyCollection<string> StringValues
        {
            get
            {
                if (!this.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a single-valued attribute value from GetValues");
                }

                return this.values.Cast<string>().ToList().AsReadOnly();
            }
        }

        public ReadOnlyCollection<long> IntegerValues
        {
            get
            {
                if (!this.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a single-valued attribute value from GetValues");
                }

                return this.values.Cast<long>().ToList().AsReadOnly();
            }
        }

        public ReadOnlyCollection<byte[]> BinaryValues
        {
            get
            {
                if (!this.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a single-valued attribute value from GetValues");
                }

                return this.values.Cast<byte[]>().ToList().AsReadOnly();
            }
        }

        public ReadOnlyCollection<UniqueIdentifier> ReferenceValues
        {
            get
            {
                if (!this.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a single-valued attribute value from GetValues");
                }

                return this.values.Cast<UniqueIdentifier>().ToList().AsReadOnly();
            }
        }

        public ReadOnlyCollection<DateTime> DateTimeValues
        {
            get
            {
                if (!this.IsMultivalued)
                {
                    throw new InvalidOperationException("Cannot get a single-valued attribute value from GetValues");
                }

                return this.values.Cast<DateTime>().ToList().AsReadOnly();
            }
        }

        internal IList<string> GetSerializationValues()
        {
            List<string> values = new List<string>();

            if (this.IsMultivalued)
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

        public static bool operator !=(AttributeValue a, object b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            if (this.IsMultivalued)
            {
                return this.values.Select(t => t.ToSmartString()).ToCommaSeparatedString();
            }
            else
            {
                return this.value.ToSmartString();
            }
        }
    }
}


