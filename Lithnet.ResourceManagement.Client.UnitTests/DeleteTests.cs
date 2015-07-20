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
    public class DeleteTests
    {
        [TestMethod]
        public void DeleteTest1()
        {
            ResourceClient c = new ResourceClient();
            UniqueIdentifier user = new UniqueIdentifier("705176b7-368f-47f9-a7ee-5daf485d1ff5");
            c.Open();

            c.Delete(user);
        }

        [TestMethod]
        public void CompositeDeleteTest()
        {
            ResourceClient c = new ResourceClient();
            UniqueIdentifier user1 = new UniqueIdentifier("46b37282-ad7e-4800-a466-b6a33b4af462");
            UniqueIdentifier user2 = new UniqueIdentifier("671533cb-db0a-4736-91f8-231fdafdf10a");
            c.Open();

            c.Delete(new List<UniqueIdentifier>() { user1, user2 });
        }

    }
}
