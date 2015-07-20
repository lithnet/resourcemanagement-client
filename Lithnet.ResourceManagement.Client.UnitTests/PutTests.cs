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
    public class PutTests
    {
        [TestMethod]
        public void PutTest1()
        {
            ResourceClient c = new ResourceClient();
            //UniqueIdentifier builtInAdmin = new UniqueIdentifier("7fb2b853-24f0-4498-9534-4e10589723c4");
            UniqueIdentifier builtInAdmin = new UniqueIdentifier("64f62191-b255-443b-bbe4-491a66300725");
            c.Open();

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

            r["AccountName"].SetValue("rnewing2");

            Debug.WriteLine("Pending changes after value mod");

            foreach (KeyValuePair<string, List<AttributeValueChange>> kvp in r.PendingChanges)
            {
                foreach (AttributeValueChange change in kvp.Value)
                {
                    Debug.WriteLine("{0}:{1}:{2}", kvp.Key, change.ChangeType, change.Value.ToSmartString());
                }
            }

            r.Save();

            Debug.WriteLine("Pending changes after commit");

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
