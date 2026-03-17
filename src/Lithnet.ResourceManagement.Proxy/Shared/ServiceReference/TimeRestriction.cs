using System;
using System.Globalization;
using System.Xml;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable]
    public class TimeRestriction
    {
        public TimeRestriction()
            : this(DateTime.UtcNow)
        {
        }

        public TimeRestriction(DateTime atTime)
        {
            this.Begin = this.End = atTime;
        }

        public TimeRestriction(DateTime begin, DateTime end)
        {
            this.Begin = begin;
            this.End = end;
        }

        internal static string ToString(TimeRestriction timeRestriction, string target)
        {
            if (timeRestriction == null)
            {
                return target;
            }

            if (timeRestriction.Begin != timeRestriction.End)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}({1}, '{2}', '{3}')", new object[] {"betweenTime", target, XmlConvert.ToString(timeRestriction.Begin, XmlDateTimeSerializationMode.Utc), XmlConvert.ToString(timeRestriction.End, XmlDateTimeSerializationMode.Utc)});
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}({1}, '{2}')", new object[] {"atTime", target, XmlConvert.ToString(timeRestriction.Begin, XmlDateTimeSerializationMode.Utc)});
        }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }
    }
}