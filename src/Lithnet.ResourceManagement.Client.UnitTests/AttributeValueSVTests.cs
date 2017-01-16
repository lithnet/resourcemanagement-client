using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.ResourceManagement.WebServices;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class AttributeValueSVTests
    {
        // Single-valued string 

        [TestMethod]
        public void TestSVAddStringWithNullInitialValue()
        {
            object testValue = "myvalue";
            object initialValue = null;
            object expectedValue = "myvalue";

            this.ExecuteSVTestAdd(AttributeType.String, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVSetStringWithNullInitialValue()
        {
            object testValue = "myvalue";
            object initialValue = null;
            object expectedValue = "myvalue";

            this.ExecuteSVTestSet(AttributeType.String, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVRemoveStringWithNullInitalValue()
        {
            object testValue = "myvalue";
            object initialValue = null;
            object expectedValue = null;

            this.ExecuteSVTestRemove(AttributeType.String, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVAddStringWithInitialValue()
        {
            object testValue = "myvalue";
            object initialValue = "initialvalue";
            object expectedValue = "myvalue";

            this.ExecuteSVTestAdd(AttributeType.String, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVSetStringWithInitialValue()
        {
            object testValue = "myvalue";
            object initialValue = "initialvalue";
            object expectedValue = "myvalue";

            this.ExecuteSVTestSet(AttributeType.String, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVRemoveStringWithInitialValue()
        {
            object testValue = "myvalue";
            object initialValue = "myvalue";
            object expectedValue = null;

            this.ExecuteSVTestRemove(AttributeType.String, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        // Single-valued long

        [TestMethod]
        public void TestSVAddLongWithNullInitialValue()
        {
            object testValue = 1L;
            object initialValue = null;
            object expectedValue = 1L;

            this.ExecuteSVTestAdd(AttributeType.Integer, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVSetLongWithNullInitialValue()
        {
            object testValue = 1L;
            object initialValue = null;
            object expectedValue = 1L;

            this.ExecuteSVTestSet(AttributeType.Integer, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVRemoveLongWithNullInitalValue()
        {
            object testValue = 1L;
            object initialValue = null;
            object expectedValue = null;

            this.ExecuteSVTestRemove(AttributeType.Integer, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVAddLongWithInitialValue()
        {
            object testValue = 1L;
            object initialValue = 2L;
            object expectedValue = 1L;

            this.ExecuteSVTestAdd(AttributeType.Integer, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVSetLongWithInitialValue()
        {
            object testValue = 1L;
            object initialValue = 2L;
            object expectedValue = 1L;

            this.ExecuteSVTestSet(AttributeType.Integer, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVRemoveLongWithInitialValue()
        {
            object testValue = 1L;
            object initialValue = 1L;
            object expectedValue = null;

            this.ExecuteSVTestRemove(AttributeType.Integer, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        // Single-valued bool

        [TestMethod]
        public void TestSVAddBooleanWithNullInitialValue()
        {
            object testValue = true;
            object initialValue = null;
            object expectedValue = true;

            this.ExecuteSVTestAdd(AttributeType.Boolean, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVSetBooleanWithNullInitialValue()
        {
            object testValue = true;
            object initialValue = null;
            object expectedValue = true;

            this.ExecuteSVTestSet(AttributeType.Boolean, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVRemoveBooleanWithNullInitalValue()
        {
            object testValue = true;
            object initialValue = null;
            object expectedValue = null;

            this.ExecuteSVTestRemove(AttributeType.Boolean, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVAddBooleanWithInitialValue()
        {
            object testValue = true;
            object initialValue = false;
            object expectedValue = true;

            this.ExecuteSVTestAdd(AttributeType.Boolean, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVSetBooleanWithInitialValue()
        {
            object testValue = true;
            object initialValue = false;
            object expectedValue = true;

            this.ExecuteSVTestSet(AttributeType.Boolean, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVRemoveBooleanWithInitialValue()
        {
            object testValue = true;
            object initialValue = true;
            object expectedValue = null;

            this.ExecuteSVTestRemove(AttributeType.Boolean, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        // Single-valued DateTime

        [TestMethod]
        public void TestSVAddDateTimeWithNullInitialValue()
        {
            object testValue = DateTime.Now;
            object initialValue = null;
            object expectedValue = testValue;

            this.ExecuteSVTestAdd(AttributeType.DateTime, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVSetDateTimeWithNullInitialValue()
        {
            object testValue = DateTime.Now;
            object initialValue = null;
            object expectedValue = testValue;

            this.ExecuteSVTestSet(AttributeType.DateTime, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVRemoveDateTimeWithNullInitalValue()
        {
            object testValue = DateTime.Now;
            object initialValue = null;
            object expectedValue = null;

            this.ExecuteSVTestRemove(AttributeType.DateTime, initialValue, testValue, expectedValue, 0);
        }

        [TestMethod]
        public void TestSVAddDateTimeWithInitialValue()
        {
            object testValue = DateTime.Now;
            object initialValue = DateTime.Now.AddDays(1);
            object expectedValue = testValue;

            this.ExecuteSVTestAdd(AttributeType.DateTime, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVSetDateTimeWithInitialValue()
        {
            object testValue = DateTime.Now;
            object initialValue = DateTime.Now.AddDays(1);
            object expectedValue = testValue;

            this.ExecuteSVTestSet(AttributeType.DateTime, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVRemoveDateTimeWithInitialValue()
        {
            object testValue = DateTime.Now;
            object initialValue = testValue;
            object expectedValue = null;

            this.ExecuteSVTestRemove(AttributeType.DateTime, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        // Single-valued Reference attributes

        [TestMethod]
        public void TestSVAddUniqueIdentifierWithNullInitialValue()
        {
            UniqueIdentifier testValue = new UniqueIdentifier("e945055f-96e7-431b-902f-e1ebd52d9253");
            UniqueIdentifier initialValue = null;
            UniqueIdentifier expectedValue = new UniqueIdentifier("e945055f-96e7-431b-902f-e1ebd52d9253");

            this.ExecuteSVTestAdd(AttributeType.Reference, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVSetUniqueIdentifierWithNullInitialValue()
        {
            UniqueIdentifier testValue = new UniqueIdentifier("e945055f-96e7-431b-902f-e1ebd52d9253");
            UniqueIdentifier initialValue = null;
            UniqueIdentifier expectedValue = new UniqueIdentifier("e945055f-96e7-431b-902f-e1ebd52d9253");

            this.ExecuteSVTestSet(AttributeType.Reference, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVRemoveUniqueIdentifierWithNullInitalValue()
        {
            UniqueIdentifier testValue = new UniqueIdentifier("e945055f-96e7-431b-902f-e1ebd52d9253");
            UniqueIdentifier initialValue = null;
            UniqueIdentifier expectedValue = null;

            this.ExecuteSVTestRemove(AttributeType.Reference, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVAddUniqueIdentifierWithInitialValue()
        {
            UniqueIdentifier testValue = new UniqueIdentifier("e945055f-96e7-431b-902f-e1ebd52d9253");
            UniqueIdentifier initialValue = new UniqueIdentifier("ba69ce24-3bac-48bb-8f0d-b82932716913");
            UniqueIdentifier expectedValue = new UniqueIdentifier("e945055f-96e7-431b-902f-e1ebd52d9253");

            this.ExecuteSVTestAdd(AttributeType.Reference, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVSetUniqueIdentifierWithInitialValue()
        {
            UniqueIdentifier testValue = new UniqueIdentifier("e945055f-96e7-431b-902f-e1ebd52d9253");
            UniqueIdentifier initialValue = new UniqueIdentifier("ba69ce24-3bac-48bb-8f0d-b82932716913");
            UniqueIdentifier expectedValue = new UniqueIdentifier("e945055f-96e7-431b-902f-e1ebd52d9253");

            this.ExecuteSVTestSet(AttributeType.Reference, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVRemoveUniqueIdentifierWithInitialValue()
        {
            UniqueIdentifier testValue = new UniqueIdentifier("e945055f-96e7-431b-902f-e1ebd52d9253");
            UniqueIdentifier initialValue = new UniqueIdentifier("e945055f-96e7-431b-902f-e1ebd52d9253");
            UniqueIdentifier expectedValue = null;
          
            this.ExecuteSVTestRemove(AttributeType.Reference, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        // Single-valued binary attributes


        [TestMethod]
        public void TestSVAddBinaryWithNullInitialValue()
        {
            byte[] testValue = new byte[4] { 0x00, 0x01, 0x03, 0x04 };
            byte[] initialValue = null;
            byte[] expectedValue = new byte[4] { 0x00, 0x01, 0x03, 0x04 };

            this.ExecuteSVTestAdd(AttributeType.Binary, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVSetBinaryWithNullInitialValue()
        {
            byte[] testValue = new byte[4] { 0x00, 0x01, 0x03, 0x04 };
            byte[] initialValue = null;
            byte[] expectedValue = new byte[4] { 0x00, 0x01, 0x03, 0x04 };

            this.ExecuteSVTestSet(AttributeType.Binary, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVRemoveBinaryWithNullInitalValue()
        {
            byte[] testValue = new byte[4] { 0x00, 0x01, 0x03, 0x04 };
            byte[] initialValue = null;
            byte[] expectedValue = null;

            this.ExecuteSVTestRemove(AttributeType.Binary, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVAddBinaryWithInitialValue()
        {
            byte[] testValue = new byte[4] { 0x00, 0x01, 0x03, 0x04 };
            byte[] initialValue = new byte[4] { 0x03, 0x04, 0x05, 0x06 };
            byte[] expectedValue = new byte[4] { 0x00, 0x01, 0x03, 0x04 };

            this.ExecuteSVTestAdd(AttributeType.Binary, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVSetBinaryWithInitialValue()
        {
            byte[] testValue = new byte[4] { 0x00, 0x01, 0x03, 0x04 };
            byte[] initialValue = new byte[4] { 0x03, 0x04, 0x05, 0x06 };
            byte[] expectedValue = new byte[4] { 0x00, 0x01, 0x03, 0x04 };

            this.ExecuteSVTestSet(AttributeType.Binary, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        [TestMethod]
        public void TestSVRemoveBinaryWithInitialValue()
        {
            byte[] testValue = new byte[4] { 0x00, 0x01, 0x03, 0x04 };
            byte[] initialValue = new byte[4] { 0x00, 0x01, 0x03, 0x04 };
            byte[] expectedValue = null;

            this.ExecuteSVTestRemove(AttributeType.Binary, initialValue, testValue, expectedValue, ModeType.Modify);
        }

        private void ExecuteSVTestAdd(AttributeType type, object initialValue, object testValue, object expectedValue, ModeType changeType)
        {
            AttributeTypeDefinition attribute = new AttributeTypeDefinition("test", type, false, false, false);
            AttributeValue value = new AttributeValue(attribute, initialValue);
            value.AddValue(testValue);
            
            if (testValue is byte[])
            {
                CollectionAssert.AreEqual((byte[])(expectedValue), (byte[])value.Value);
            }
            else
            {
                Assert.AreEqual(expectedValue, value.Value);
            }

            Assert.AreEqual(1, value.ValueChanges.Count);

            AttributeValueChange change = value.ValueChanges.First();

            Assert.AreEqual(changeType, change.ChangeType);

            if (testValue is byte[])
            {
                CollectionAssert.AreEqual((byte[])(expectedValue), (byte[])change.Value);
            }
            else
            {
                Assert.AreEqual(expectedValue, change.Value);
            }
        }

        private void ExecuteSVTestSet(AttributeType type, object initialValue, object testValue, object expectedValue, ModeType changeType)
        {
            AttributeTypeDefinition attribute = new AttributeTypeDefinition("test", type, false, false, false);
            AttributeValue value = new AttributeValue(attribute, initialValue);
            value.SetValue(testValue);

            if (testValue is byte[])
            {
                CollectionAssert.AreEqual((byte[])(expectedValue), (byte[])value.Value);
            }
            else
            {
                Assert.AreEqual(expectedValue, value.Value);
            }

            Assert.AreEqual(1, value.ValueChanges.Count);

            AttributeValueChange change = value.ValueChanges.First();

            Assert.AreEqual(changeType, change.ChangeType);

            if (testValue is byte[])
            {
                CollectionAssert.AreEqual((byte[])(expectedValue), (byte[])change.Value);
            }
            else
            {
                Assert.AreEqual(expectedValue, change.Value);
            }
        }
        
        private void ExecuteSVTestRemove(AttributeType type, object initialValue, object testValue, object expectedValue, ModeType changeType)
        {
            AttributeTypeDefinition attribute = new AttributeTypeDefinition("test", type, false, false, false);
            AttributeValue value = new AttributeValue(attribute, initialValue);
            value.RemoveValue(testValue);
            Assert.AreEqual(expectedValue, value.Value);

            if (initialValue == null)
            {
                Assert.AreEqual(0, value.ValueChanges.Count);
            }
            else
            {
                Assert.AreEqual(1, value.ValueChanges.Count);
                AttributeValueChange change = value.ValueChanges.First();
                Assert.AreEqual(changeType, change.ChangeType);

                if (testValue is byte[])
                {
                    CollectionAssert.AreEqual((byte[])(expectedValue), (byte[])change.Value);
                }
                else
                {
                    Assert.AreEqual(null, change.Value);
                }
            }
        }
    }
}
