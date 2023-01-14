using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable]
    public class UniqueIdentifier
    {
        private Guid? guidField;
        private static Regex GuidFormat = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);
        private string valueField;

        public UniqueIdentifier()
        {
        }

        public UniqueIdentifier(Guid guid)
        {
            this.valueField = guid.ToString();
            this.guidField = new Guid?(guid);
        }

        public UniqueIdentifier(string value)
        {
            this.Value = value;
        }

        internal static UniqueIdentifier Convert(object value)
        {
            UniqueIdentifier identifier = value as UniqueIdentifier;
            if ((null == identifier) && (value != null))
            {
                string str = value as string;
                if (str != null)
                {
                    return new UniqueIdentifier(str);
                }
                try
                {
                    Guid guid = (Guid)value;
                    identifier = new UniqueIdentifier(guid);
                }
                catch (InvalidCastException)
                {
                }
            }
            return identifier;
        }

        public override bool Equals(object obj)
        {
            UniqueIdentifier identifier = obj as UniqueIdentifier;
            return (identifier == this);
        }

        public Guid GetGuid()
        {
            if (!this.guidField.HasValue)
            {
                throw new InvalidOperationException();
            }
            return this.guidField.Value;
        }

        public override int GetHashCode()
        {
            return this.valueField.GetHashCode();
        }

        public static bool operator ==(UniqueIdentifier id1, UniqueIdentifier id2)
        {
            if (!object.ReferenceEquals(id1, id2))
            {
                if ((null == id1) || (null == id2))
                {
                    return false;
                }
                Guid? guidField = id1.guidField;
                Guid? nullable2 = id2.guidField;
                if (guidField.HasValue != nullable2.HasValue)
                {
                    return false;
                }
                if (guidField.HasValue)
                {
                    return (guidField.GetValueOrDefault() == nullable2.GetValueOrDefault());
                }
            }

            return true;
        }

        public static explicit operator Guid(UniqueIdentifier value)
        {
            if (null == value)
            {
                return Guid.Empty;
            }
            return value.GetGuid();
        }

        public static explicit operator UniqueIdentifier(string value)
        {
            if (value != null)
            {
                return new UniqueIdentifier(value);
            }
            return null;
        }

        public static explicit operator UniqueIdentifier(UniqueId value)
        {
            Guid guid;
            if (null == value)
            {
                return null;
            }
            if (value.TryGetGuid(out guid))
            {
                return new UniqueIdentifier(guid);
            }
            return new UniqueIdentifier(value.ToString());
        }

        public static implicit operator UniqueIdentifier(Guid value)
        {
            return new UniqueIdentifier(value);
        }

        public static bool operator !=(UniqueIdentifier id1, UniqueIdentifier id2)
        {
            return !(id1 == id2);
        }

        public override string ToString()
        {
            if (this.guidField.HasValue)
            {
                return new UniqueId(this.guidField.Value).ToString();
            }
            if (string.IsNullOrEmpty(this.Value))
            {
                return string.Empty;
            }
            return new UniqueId(this.Value).ToString();
        }

        public string ToString(bool usePrefix)
        {
            if (usePrefix)
            {
                return this.ToString();
            }
            if (this.guidField.HasValue)
            {
                return this.guidField.Value.ToString();
            }
            return this.Value.ToString();
        }

        public bool TryGetGuid(out Guid guid)
        {
            guid = Guid.Empty;
            if (this.guidField.HasValue)
            {
                guid = this.guidField.Value;
                return true;
            }
            return false;
        }

        public bool IsGuid => this.guidField.HasValue;

        public string Value
        {
            get => this.valueField;
            set
            {
                string input = value.StartsWith("urn:uuid:", StringComparison.Ordinal) ? value.Remove(0, "urn:uuid:".Length) : value;
                if (GuidFormat.IsMatch(input))
                {
                    this.valueField = input;
                    this.guidField = new Guid(input);
                }
                else
                {
                    this.valueField = value;
                    this.guidField = null;
                }
            }
        }
    }
}
