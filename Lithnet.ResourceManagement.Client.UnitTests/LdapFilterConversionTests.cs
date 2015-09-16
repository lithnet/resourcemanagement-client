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
            //string filter = @"ldap:///o=Monash University, c=au??sub?(objectClass=monashStudent)";
            ////LdapFilterToXPathExpression.ConvertLdapFilter(filter);

            //filter = @"ldap:///o=Monash University, c=au??sub?(&(ou=Library)(mail=*)(uid=*))";
            ////filter = @"ldap:///o=Monash University, c=au??sub?(&(&(ou=Off-Campus Learning Centre)(mail=*))(uid=*))";
            //filter = @"ldap:///o=Monash University, c=au??sub?(&(&(l=Gippsland)(mail=*))(uid=*))";
            //filter = @"ldap:///o=Monash University, c=au??sub?(&(|(&(|(&(mail=*)(objectClass=monashPerson)(buildingname=82)(ou=Faculty of Engineering))))(monashlistoptin=New Horizons All Staff))(!(monashlistoptout=New Horizons All Staff)))";

            StringBuilder b = new StringBuilder();
            foreach (string filter in System.IO.File.ReadAllLines(@"D:\temp\ldapfilters.txt"))
            {
                XPathExpression e = LdapFilterConverter.ConvertLdapFilter(filter);
                b.AppendLine(e.ToString());   
            }

            System.IO.File.WriteAllText(@"D:\temp\xpathfilters.txt", b.ToString());
        }
    }
}