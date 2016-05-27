using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using Lithnet.ResourceManagement.Client;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;
using Microsoft.ResourceManagement.WebServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class LdapFilterConversionTests
    {
        [TestMethod]
        public void Test1()
        {
            //StringBuilder b = new StringBuilder();
            //foreach (string filter in System.IO.File.ReadAllLines(@"D:\temp\ldapfilters.txt"))
            //{
            //    XPathExpression e = LdapFilterConverter.ConvertLdapFilter(filter);
            //    b.AppendLine(e.ToString());   
            //}

            //System.IO.File.WriteAllText(@"D:\temp\xpathfilters.txt", b.ToString());
        }
    }
}