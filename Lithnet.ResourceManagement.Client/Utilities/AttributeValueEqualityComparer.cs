using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Lithnet.ResourceManagement.Client
{
    internal class AttributeValueEqualityComparer : IEqualityComparer<object>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
        }

        public bool Equals(AttributeValue x, AttributeValue y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (x.Attribute.SystemName != y.Attribute.SystemName)
            {
                return false;
            }

            if (x.Attribute.IsMultivalued)
            {
                return this.Equals((IList)x.Value, (IList)y.Value);
            }
            else
            {
                return this.Equals(x.Value, y.Value);
            }
        }

        public new bool Equals(object x, object y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            IList list1 = x as IList;
            IList list2 = y as IList;

            if (list1 != null && list2 != null)
            {
                return this.Equals(list1, list2);
            }

            byte[] byte1 = x as byte[];
            byte[] byte2 = y as byte[];

            if (byte1 != null && byte2 != null)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(byte1, byte2);
            }

            string string1 = x.ToString();
            string string2 = y.ToString();

            return string1.Equals(string2);
        }

        public bool Equals(IList x, IList y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.Count != y.Count)
            {
                return false;
            }

            foreach (object xValue in x)
            {
                if (!y.OfType<object>().Any(t => this.Equals(t, xValue)))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return obj.GetHashCode();
            }
        }
    }
}
