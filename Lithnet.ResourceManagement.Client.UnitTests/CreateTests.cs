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
    public class CreateTests
    {
        [TestMethod]
        public void CreateWithoutObjectID()
        {
            ResourceFactoryClient c = new ResourceFactoryClient();
            //UniqueIdentifier builtInAdmin = new UniqueIdentifier("705176b7-368f-47f9-a7ee-5daf485d1ff5");
            c.Open();

            ResourceObject r = new ResourceObject("Person");
            r.Attributes["AccountName"].SetValue("zzztest1");
            r.Attributes["Domain"].SetValue(Environment.GetEnvironmentVariable("USERDOMAIN"));
            r.Attributes["organizationalRelationships"].SetValue(new List<string>() { "OR0001", "OR0002" });

            c.Create(r);
        }

    }
}
