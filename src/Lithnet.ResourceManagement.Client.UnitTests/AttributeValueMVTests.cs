﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class AttributeValueMVTests
    {
        AttributeValueEqualityComparer comparer = new AttributeValueEqualityComparer();

        // Multivalued string 

        [TestMethod]
        public void TestMVAddStringWithNullInitialValue()
        {
            List<object> testValues = new List<object>() { "myvalue1", "myvalue2" };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>() { "myvalue1", "myvalue2" };

            this.ExecuteMVTestAdd(AttributeType.String, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVBlindRemoveString()
        {
            List<object> testValues = new List<object>() { "myvalue1" };
            this.ExecuteMVTestRemoveBlind(AttributeType.String, testValues);
        }

        [TestMethod]
        public void TestMVSetStringWithNullInitialValue()
        {
            List<object> testValues = new List<object>() { "myvalue1", "myvalue2" };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>() { "myvalue1", "myvalue2" };

            this.ExecuteMVTestSet(AttributeType.String, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVRemoveStringWithNullInitalValue()
        {
            List<object> testValues = new List<object>() { "myvalue1", "myvalue2" };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>();

            this.ExecuteMVTestRemove(AttributeType.String, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVAddStringWithInitialValue()
        {
            List<object> testValues = new List<object>() { "myvalue1", "myvalue2" };
            List<object> initialValues = new List<object>() { "myvalue3", "myvalue4" };
            List<object> expectedValues = new List<object>() { "myvalue1", "myvalue2", "myvalue3", "myvalue4" };

            this.ExecuteMVTestAdd(AttributeType.String, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVSetStringWithInitialValue()
        {
            List<object> testValues = new List<object>() { "myvalue1", "myvalue2" };
            List<object> initialValues = new List<object>() { "myvalue3", "myvalue4" };
            List<object> expectedValues = new List<object>() { "myvalue1", "myvalue2" };

            this.ExecuteMVTestSet(AttributeType.String, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVRemoveStringWithInitialValue()
        {
            List<object> testValues = new List<object>() { "myvalue1", "myvalue2" };
            List<object> initialValues = new List<object>() { "myvalue1", "myvalue2", "myvalue3" };
            List<object> expectedValues = new List<object>() { "myvalue3" };

            this.ExecuteMVTestRemove(AttributeType.String, initialValues, testValues, expectedValues);
        }

        // Multivalued long 

        [TestMethod]
        public void TestMVAddIntegerWithNullInitialValue()
        {
            List<object> testValues = new List<object>() { 1, 2 };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>() { 1, 2 };

            this.ExecuteMVTestAdd(AttributeType.Integer, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVSetIntegerWithNullInitialValue()
        {
            List<object> testValues = new List<object>() { 1, 2 };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>() { 1, 2 };

            this.ExecuteMVTestSet(AttributeType.Integer, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVRemoveIntegerWithNullInitalValue()
        {
            List<object> testValues = new List<object>() { 1, 2 };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>();

            this.ExecuteMVTestRemove(AttributeType.Integer, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVAddIntegerWithInitialValue()
        {
            List<object> testValues = new List<object>() { 1, 2 };
            List<object> initialValues = new List<object>() { 3, 4 };
            List<object> expectedValues = new List<object>() { 1, 2, 3, 4 };

            this.ExecuteMVTestAdd(AttributeType.Integer, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVSetIntegerWithInitialValue()
        {
            List<object> testValues = new List<object>() { 1, 2 };
            List<object> initialValues = new List<object>() { 3, 4 };
            List<object> expectedValues = new List<object>() { 1, 2 };

            this.ExecuteMVTestSet(AttributeType.Integer, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVRemoveIntegerWithInitialValue()
        {
            List<object> testValues = new List<object>() { 1, 2 };
            List<object> initialValues = new List<object>() { 1, 2, 3 };
            List<object> expectedValues = new List<object>() { 3 };

            this.ExecuteMVTestRemove(AttributeType.Integer, initialValues, testValues, expectedValues);
        }

        // Multivalued DateTime 

        [TestMethod]
        public void TestMVAddDateTimeWithNullInitialValue()
        {
            DateTime value1 = DateTime.Now;
            DateTime value2 = DateTime.Now.AddDays(1);
            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>() { value1, value2 };

            this.ExecuteMVTestAdd(AttributeType.DateTime, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVSetDateTimeWithNullInitialValue()
        {
            DateTime value1 = DateTime.Now;
            DateTime value2 = DateTime.Now.AddDays(1);
            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>() { value1, value2 };

            this.ExecuteMVTestSet(AttributeType.DateTime, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVRemoveDateTimeWithNullInitalValue()
        {
            DateTime value1 = DateTime.Now;
            DateTime value2 = DateTime.Now.AddDays(1);
            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>();

            this.ExecuteMVTestRemove(AttributeType.DateTime, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVAddDateTimeWithInitialValue()
        {
            DateTime value1 = DateTime.Now;
            DateTime value2 = DateTime.Now.AddDays(1);
            DateTime value3 = DateTime.Now.AddDays(2);
            DateTime value4 = DateTime.Now.AddDays(3);
            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>() { value3, value4 };
            List<object> expectedValues = new List<object>() { value1, value2, value3, value4 };

            this.ExecuteMVTestAdd(AttributeType.DateTime, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVSetDateTimeWithInitialValue()
        {
            DateTime value1 = DateTime.Now;
            DateTime value2 = DateTime.Now.AddDays(1);
            DateTime value3 = DateTime.Now.AddDays(2);
            DateTime value4 = DateTime.Now.AddDays(3);
            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>() { value3, value4 };
            List<object> expectedValues = new List<object>() { value1, value2 };

            this.ExecuteMVTestSet(AttributeType.DateTime, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVRemoveDateTimeWithInitialValue()
        {
            DateTime value1 = DateTime.Now;
            DateTime value2 = DateTime.Now.AddDays(1);
            DateTime value3 = DateTime.Now.AddDays(2);
            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>() { value1, value2, value3 };
            List<object> expectedValues = new List<object>() { value3 };

            this.ExecuteMVTestRemove(AttributeType.DateTime, initialValues, testValues, expectedValues);
        }

        // Multivalued UniqueIdentifier 

        [TestMethod]
        public void TestMVAddReferenceWithNullInitialValue()
        {
            UniqueIdentifier value1 = new UniqueIdentifier("0dcafc9f-d4cf-4754-b10d-4716d9a05be6");
            UniqueIdentifier value2 = new UniqueIdentifier("9626643b-f4df-45a4-b080-df1d9a057bca");
            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>() { value1.ToString(), value2.ToString() };

            this.ExecuteMVTestAdd(AttributeType.Reference, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVSetReferenceWithNullInitialValue()
        {
            UniqueIdentifier value1 = new UniqueIdentifier("0dcafc9f-d4cf-4754-b10d-4716d9a05be6");
            UniqueIdentifier value2 = new UniqueIdentifier("9626643b-f4df-45a4-b080-df1d9a057bca");
            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>() { value1.ToString(), value2.ToString() };

            this.ExecuteMVTestSet(AttributeType.Reference, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVRemoveReferenceWithNullInitalValue()
        {
            UniqueIdentifier value1 = new UniqueIdentifier("0dcafc9f-d4cf-4754-b10d-4716d9a05be6");
            UniqueIdentifier value2 = new UniqueIdentifier("9626643b-f4df-45a4-b080-df1d9a057bca");
            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>();

            this.ExecuteMVTestRemove(AttributeType.Reference, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVAddReferenceWithInitialValue()
        {
            UniqueIdentifier value1 = new UniqueIdentifier("0dcafc9f-d4cf-4754-b10d-4716d9a05be6");
            UniqueIdentifier value2 = new UniqueIdentifier("9626643b-f4df-45a4-b080-df1d9a057bca");
            UniqueIdentifier value3 = new UniqueIdentifier("204a45d7-b6bd-4835-ae01-8245a9a710b2");
            UniqueIdentifier value4 = new UniqueIdentifier("1a9de914-ef41-4cf6-84ad-7515e704cbf5");
            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>() { value3, value4 };
            List<object> expectedValues = new List<object>() { value1.ToString(), value2.ToString(), value3.ToString(), value4.ToString() };

            this.ExecuteMVTestAdd(AttributeType.Reference, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVSetReferenceWithInitialValue()
        {
            UniqueIdentifier value1 = new UniqueIdentifier("0dcafc9f-d4cf-4754-b10d-4716d9a05be6");
            UniqueIdentifier value2 = new UniqueIdentifier("9626643b-f4df-45a4-b080-df1d9a057bca");
            UniqueIdentifier value3 = new UniqueIdentifier("204a45d7-b6bd-4835-ae01-8245a9a710b2");
            UniqueIdentifier value4 = new UniqueIdentifier("1a9de914-ef41-4cf6-84ad-7515e704cbf5");
            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>() { value3, value4 };
            List<object> expectedValues = new List<object>() { value1.ToString(), value2.ToString() };

            this.ExecuteMVTestSet(AttributeType.Reference, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVRemoveReferenceWithInitialValue()
        {
            UniqueIdentifier value1 = new UniqueIdentifier("0dcafc9f-d4cf-4754-b10d-4716d9a05be6");
            UniqueIdentifier value2 = new UniqueIdentifier("9626643b-f4df-45a4-b080-df1d9a057bca");
            UniqueIdentifier value3 = new UniqueIdentifier("204a45d7-b6bd-4835-ae01-8245a9a710b2");
            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>() { value1, value2, value3 };
            List<object> expectedValues = new List<object>() { value3.ToString() };

            this.ExecuteMVTestRemove(AttributeType.Reference, initialValues, testValues, expectedValues);
        }

        // Multivalued byte[] 

        [TestMethod]
        public void TestMVAddBinaryWithNullInitialValue()
        {
            byte[] value1 = new byte[4] { 0x01, 0x02, 0x03, 0x04 };
            byte[] value2 = new byte[4] { 0x05, 0x06, 0x07, 0x08 };

            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>() { value1.Clone(), value2.Clone() };

            this.ExecuteMVTestAdd(AttributeType.Binary, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVSetBinaryWithNullInitialValue()
        {
            byte[] value1 = new byte[4] { 0x01, 0x02, 0x03, 0x04 };
            byte[] value2 = new byte[4] { 0x05, 0x06, 0x07, 0x08 };

            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>() { value1.Clone(), value2.Clone() };

            this.ExecuteMVTestSet(AttributeType.Binary, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVRemoveBinaryWithNullInitalValue()
        {
            byte[] value1 = new byte[4] { 0x01, 0x02, 0x03, 0x04 };
            byte[] value2 = new byte[4] { 0x05, 0x06, 0x07, 0x08 };

            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>();
            List<object> expectedValues = new List<object>();

            this.ExecuteMVTestRemove(AttributeType.Binary, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVAddBinaryWithInitialValue()
        {
            byte[] value1 = new byte[4] { 0x01, 0x02, 0x03, 0x04 };
            byte[] value2 = new byte[4] { 0x05, 0x06, 0x07, 0x08 };
            byte[] value3 = new byte[4] { 0x09, 0x0A, 0x0B, 0x0C };
            byte[] value4 = new byte[4] { 0x0D, 0x0E, 0x0F, 0x10 };

            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>() { value3, value4 };
            List<object> expectedValues = new List<object>() { value1.Clone(), value2.Clone(), value3.Clone(), value4.Clone() };

            this.ExecuteMVTestAdd(AttributeType.Binary, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVSetBinaryWithInitialValue()
        {
            byte[] value1 = new byte[4] { 0x01, 0x02, 0x03, 0x04 };
            byte[] value2 = new byte[4] { 0x05, 0x06, 0x07, 0x08 };
            byte[] value3 = new byte[4] { 0x09, 0x0A, 0x0B, 0x0C };
            byte[] value4 = new byte[4] { 0x0D, 0x0E, 0x0F, 0x10 };

            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>() { value3, value4 };
            List<object> expectedValues = new List<object>() { value1.Clone(), value2.Clone() };

            this.ExecuteMVTestSet(AttributeType.Binary, initialValues, testValues, expectedValues);
        }

        [TestMethod]
        public void TestMVRemoveBinaryWithInitialValue()
        {
            byte[] value1 = new byte[4] { 0x01, 0x02, 0x03, 0x04 };
            byte[] value2 = new byte[4] { 0x05, 0x06, 0x07, 0x08 };
            byte[] value3 = new byte[4] { 0x09, 0x0A, 0x0B, 0x0C };

            List<object> testValues = new List<object>() { value1, value2 };
            List<object> initialValues = new List<object>() { value1, value2, value3 };
            List<object> expectedValues = new List<object>() { value3.Clone() };
            
            this.ExecuteMVTestRemove(AttributeType.Binary, initialValues, testValues, expectedValues);
        }

        private void ExecuteMVTestAdd(AttributeType type, IList<object> initialValues, IList<object> testValues, IList<object> expectedValues)
        {
            AttributeTypeDefinition attribute = new AttributeTypeDefinition("test", type, true, false, false);
            AttributeValue value = new AttributeValue(attribute, initialValues);

            foreach (object testValue in testValues)
            {
                value.AddValue(testValue);
            }

            Assert.IsTrue(this.comparer.Equals(value.Value, expectedValues));

            List<AttributeValueChange> expectedChanges = new List<AttributeValueChange>();
            foreach (object testValue in testValues)
            {
                expectedChanges.Add(new AttributeValueChange(ModeType.Insert, testValue));
            }

            Assert.AreEqual(expectedChanges.Count, value.ValueChanges.Count);

            foreach (AttributeValueChange expectedChange in expectedChanges)
            {
                if (!value.ValueChanges.Any(t => t.ChangeType == expectedChange.ChangeType && this.comparer.Equals(t.Value, expectedChange.Value)))
                {
                    Assert.Fail("The expectedXpath change was not found: {0}:{1}", expectedChange.ChangeType, expectedChange.Value);
                }
            }
        }

        private void ExecuteMVTestSet(AttributeType type, IList<object> initialValues, IList<object> testValues, IList<object> expectedValues)
        {
            AttributeTypeDefinition attribute = new AttributeTypeDefinition("test", type, true, false, false);
            AttributeValue value = new AttributeValue(attribute, initialValues);

            value.SetValue(testValues);

            Assert.IsTrue(this.comparer.Equals(value.Value, expectedValues));

            List<AttributeValueChange> expectedChanges = new List<AttributeValueChange>();
            foreach (object testValue in testValues)
            {
                expectedChanges.Add(new AttributeValueChange(ModeType.Insert, testValue));
            }

            foreach (object initialValue in initialValues)
            {
                expectedChanges.Add(new AttributeValueChange(ModeType.Remove, initialValue));
            }

            Assert.AreEqual(expectedChanges.Count, value.ValueChanges.Count);

            foreach (AttributeValueChange expectedChange in expectedChanges)
            {
                if (!value.ValueChanges.Any(t => t.ChangeType == expectedChange.ChangeType && this.comparer.Equals(t.Value, expectedChange.Value)))
                {
                    Assert.Fail("The expectedXpath change was not found: {0}:{1}", expectedChange.ChangeType, expectedChange.Value);
                }
            }
        }

        private void ExecuteMVTestRemove(AttributeType type, IList<object> initialValues, IList<object> testValues, IList<object> expectedValues)
        {
            AttributeTypeDefinition attribute = new AttributeTypeDefinition("test", type, true, false, false);
            AttributeValue value = new AttributeValue(attribute, initialValues);

            foreach (object testValue in testValues)
            {
                value.RemoveValue(testValue);
            }

            Assert.IsTrue(this.comparer.Equals(value.Value, expectedValues));

            List<AttributeValueChange> expectedChanges = new List<AttributeValueChange>();

            if (initialValues.Count + expectedValues.Count > 0)
            {
                foreach (object testValue in testValues)
                {
                    expectedChanges.Add(new AttributeValueChange(ModeType.Remove, testValue));
                }
            }

            Assert.AreEqual(expectedChanges.Count, value.ValueChanges.Count);

            foreach (AttributeValueChange expectedChange in expectedChanges)
            {
                if (!value.ValueChanges.Any(t => t.ChangeType == expectedChange.ChangeType && this.comparer.Equals(t.Value, expectedChange.Value)))
                {
                    Assert.Fail("The expectedXpath change was not found: {0}:{1}", expectedChange.ChangeType, expectedChange.Value);
                }
            }
        }

        private void ExecuteMVTestRemoveBlind(AttributeType type, IList<object> valuesToBlindRemove)
        {
            AttributeTypeDefinition attribute = new AttributeTypeDefinition("test", type, true, false, false);
            AttributeValue value = new AttributeValue(attribute);

            List<AttributeValueChange> expectedChanges = new List<AttributeValueChange>();

            foreach (object testValue in valuesToBlindRemove)
            {
                value.RemoveValue(testValue, true);
                expectedChanges.Add(new AttributeValueChange(ModeType.Remove, testValue));
            }

            Assert.AreEqual(expectedChanges.Count, value.ValueChanges.Count);

            foreach (AttributeValueChange expectedChange in expectedChanges)
            {
                if (!value.ValueChanges.Any(t => t.ChangeType == expectedChange.ChangeType && this.comparer.Equals(t.Value, expectedChange.Value)))
                {
                    Assert.Fail("The expectedXpath change was not found: {0}:{1}", expectedChange.ChangeType, expectedChange.Value);
                }
            }
        }
    }
}
