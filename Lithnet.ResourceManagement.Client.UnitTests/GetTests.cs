using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using Lithnet.ResourceManagement.Client;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;
using Microsoft.ResourceManagement.WebServices;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class GetTests
    {
        [TestMethod]
        public void GetObjectAllAttributesSpecified()
        {
            ResourceClient c = new ResourceClient();
            //64f62191-b255-443b-bbe4-491a66300725
            UniqueIdentifier builtInAdmin = new UniqueIdentifier("7fb2b853-24f0-4498-9534-4e10589723c4");
            //UniqueIdentifier builtInAdmin = new UniqueIdentifier("64f62191-b255-443b-bbe4-491a66300725");
            c.Open();

            List<string> attributeList = new List<string>();
            //attributeList.Add("ObjectID");
            //attributeList.Add("ObjectType");
            //attributeList.Add("AccountName");
            //attributeList.Add("organizationalRelationships");

            foreach (AttributeTypeDefinition attribute in Schema.ObjectTypes["Person"])
            {
                attributeList.Add(attribute.SystemName);
            }

            ResourceObject r = c.Get(builtInAdmin, attributeList);

            Assert.AreEqual(r.ObjectTypeName, "Person");

            Debug.WriteLine("Object drop");
            Debug.WriteLine(r.ToString());

            Debug.WriteLine("Pending changes");

            foreach (KeyValuePair<string, List<AttributeValueChange>> kvp in r.PendingChanges)
            {
                foreach (AttributeValueChange change in kvp.Value)
                {
                    Debug.WriteLine("{0}:{1}:{2}", kvp.Key, change.ChangeType, change.Value.ToSmartString());
                }
            }
        }        

        [TestMethod]
        public void GetObjectNoAttributesSpecified()
        {
            ResourceClient c = new ResourceClient();
            //64f62191-b255-443b-bbe4-491a66300725
            //UniqueIdentifier builtInAdmin = new UniqueIdentifier("7fb2b853-24f0-4498-9534-4e10589723c4");
            UniqueIdentifier builtInAdmin = new UniqueIdentifier("64f62191-b255-443b-bbe4-491a66300725");
            c.Initialize();

            ResourceObject r = c.Get(builtInAdmin);

            Assert.AreEqual(r.ObjectTypeName, "Person");

            Debug.WriteLine("Object drop");
            Debug.WriteLine(r.ToString());

            Debug.WriteLine("Pending changes");

            foreach (KeyValuePair<string, List<AttributeValueChange>> kvp in r.PendingChanges)
            {
                foreach (AttributeValueChange change in kvp.Value)
                {
                    Debug.WriteLine("{0}:{1}:{2}", kvp.Key, change.ChangeType, change.Value.ToSmartString());
                }
            }
        }
    }
}
