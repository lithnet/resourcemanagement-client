using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    internal static class Constants
    {
        public const string TestLocale = "it-IT";

        public const string UnitTestObjectTypeName = "_unitTestObject";
        public const string AttributeStringSV = "ut_svstring";
        public const string AttributeStringMV = "ut_mvstring";
        public const string AttributeIntegerSV = "ut_svinteger";
        public const string AttributeIntegerMV = "ut_mvinteger";
        public const string AttributeReferenceSV = "ut_svreference";
        public const string AttributeReferenceMV = "ut_mvreference";
        public const string AttributeTextSV = "ut_svtext";
        public const string AttributeTextMV = "ut_mvtext";
        public const string AttributeDateTimeSV = "ut_svdatetime";
        public const string AttributeDateTimeMV = "ut_mvdatetime";
        public const string AttributeBinarySV = "ut_svbinary";
        public const string AttributeBinaryMV = "ut_mvbinary";
        public const string AttributeBooleanSV = "ut_svboolean";

        public const string TestDataString1 = "testString1";
        public const string TestDataString2 = "testString2";
        public const string TestDataString3 = "testString3";

        public const long TestDataInteger1 = 4L;
        public const long TestDataInteger2 = 5L;
        public const long TestDataInteger3 = 6L;

        public const string TestDataText1 = "testText1";
        public const string TestDataText2 = "testText2";
        public const string TestDataText3 = "testText3";

        public const bool TestDataBooleanTrue = true;
        public const bool TestDataBooleanFalse = false;

        public static readonly byte[] TestDataBinary1 = new byte[4] { 0, 1, 2, 3 };
        public static readonly byte[] TestDataBinary2 = new byte[4] { 4, 5, 6, 7 };
        public static readonly byte[] TestDataBinary3 = new byte[4] { 8, 9, 10, 11 };

        // We need to round-trip these values to avoid failed unit tests due to rounding
        public static readonly DateTime TestDataDateTime1 = DateTime.Parse(DateTime.UtcNow.ToResourceManagementServiceDateFormat(true));
        public static readonly DateTime TestDataDateTime2 = DateTime.Parse(DateTime.UtcNow.AddDays(1).ToResourceManagementServiceDateFormat(true));
        public static readonly DateTime TestDataDateTime3 = DateTime.Parse(DateTime.UtcNow.AddDays(1).ToResourceManagementServiceDateFormat(true));

        public static readonly List<string> TestDataString1MV = new List<string>() { "testString4", "testString5", "testString6" };
        public static readonly List<string> TestDataString2MV = new List<string>() { "testString7", "testString8", "testString9" };

        public static readonly List<long> TestDataInteger1MV = new List<long>() { 13L, 14L, 15L };
        public static readonly List<long> TestDataInteger2MV = new List<long>() { 16L, 17L, 18L };

        public static readonly List<byte[]> TestDataBinary1MV = new List<byte[]>() { new byte[4] { 12, 13, 14, 15 }, new byte[4] { 16, 17, 18, 19 }, new byte[4] { 20, 21, 22, 23 } };
        public static readonly List<byte[]> TestDataBinary2MV = new List<byte[]>() { new byte[4] { 24, 25, 26, 27 }, new byte[4] { 28, 29, 30, 31 }, new byte[4] { 32, 33, 34, 35 } };

        public static readonly List<DateTime> TestDataDateTime1MV = new List<DateTime>() {
            DateTime.Parse(DateTime.UtcNow.AddDays(3).ToResourceManagementServiceDateFormat(true)),
            DateTime.Parse(DateTime.UtcNow.AddDays(4).ToResourceManagementServiceDateFormat(true)),
            DateTime.Parse(DateTime.UtcNow.AddDays(5).ToResourceManagementServiceDateFormat(true)) };

        public static readonly List<DateTime> TestDataDateTime2MV = new List<DateTime>() {
            DateTime.Parse(DateTime.UtcNow.AddDays(6).ToResourceManagementServiceDateFormat(true)),
            DateTime.Parse(DateTime.UtcNow.AddDays(7).ToResourceManagementServiceDateFormat(true)),
            DateTime.Parse(DateTime.UtcNow.AddDays(8).ToResourceManagementServiceDateFormat(true)) };

        public static List<UniqueIdentifier> TestDataReference1MV;
        public static List<UniqueIdentifier> TestDataReference2MV;

        public static readonly List<string> TestDataText1MV = new List<string>() { "testText4", "testText5", "testText6" };
        public static readonly List<string> TestDataText2MV = new List<string>() { "testText7", "testText8", "testText9" };

        public static readonly CultureInfo TestCulture = new CultureInfo(Constants.TestLocale);

        public static UniqueIdentifier TestDataReference1;
        public static UniqueIdentifier TestDataReference2;
        public static UniqueIdentifier TestDataReference3;
        public static UniqueIdentifier TestDataReference4;
        public static UniqueIdentifier TestDataReference5;
        public static UniqueIdentifier TestDataReference6;
    }
}