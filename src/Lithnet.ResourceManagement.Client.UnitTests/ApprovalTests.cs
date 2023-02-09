﻿using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class ApprovalTests
    {
        NetworkCredential standardUserCredentials;

        public ApprovalTests()
        {
            var username = Environment.GetEnvironmentVariable("UnitTestApprovalUserName") ?? ConfigurationManager.AppSettings["UnitTestApprovalUserName"];

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("No username was provided for a standard user needed to conduct the approval tests");
            }

            var domain = Environment.GetEnvironmentVariable("UnitTestApprovalUserDomain") ?? ConfigurationManager.AppSettings["UnitTestApprovalUserDomain"];
            var password = Environment.GetEnvironmentVariable("UnitTestApprovalUserPassword") ?? ConfigurationManager.AppSettings["UnitTestApprovalUserPassword"];
            var encryptedPassword = Environment.GetEnvironmentVariable("UnitTestApprovalUserEncryptedPassword") ?? ConfigurationManager.AppSettings["UnitTestApprovalUserEncryptedPassword"];

            if (encryptedPassword != null)
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);
                var unprotectedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
                password = Encoding.ASCII.GetString(unprotectedBytes);
            }

            this.standardUserCredentials = new NetworkCredential(username, password, domain);
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestApprovalForCurrentUser(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            ResourceObject group = null;
            ResourceObject owner = null;
            ResourceObject member = null;

            string groupName = Guid.NewGuid().ToString();

            try
            {
                member = client.GetResourceByKey("Person", "AccountName", this.standardUserCredentials.UserName);
                owner = client.GetResourceByKey("Person", "AccountName", Environment.UserName);

                group = client.CreateResource("Group");
                group.SetValue("AccountName", groupName);
                group.SetValue("DisplayName", $"Unit test approval group {groupName}");
                group.SetValue("Owner", owner);
                group.SetValue("DisplayedOwner", owner);
                group.SetValue("Scope", "Global");
                group.SetValue("Type", "Security");
                group.SetValue("Domain", Environment.UserDomainName);
                group.SetValue("MembershipLocked", false);
                group.SetValue("MembershipAddWorkflow", "Owner Approval");

                client.SaveResource(group);

                UniqueIdentifier requestId = null;

                Assert.ThrowsException<AuthorizationRequiredException>(() =>
                {
                    try
                    {
                        this.AddTestUserToGroup(group, member, connectionMode);
                    }
                    catch (AuthorizationRequiredException ex)
                    {
                        if (ex.ResourceReference != null)
                        {
                            requestId = new UniqueIdentifier(ex.ResourceReference);
                        }

                        throw;
                    }
                });

                Assert.IsNotNull(requestId);

                // Make sure that the request was logged, searching specifically by our own resource ID
                var pendingApprovals = client.GetApprovals(ApprovalStatus.Pending, owner.ObjectID);
                var ourApproval = pendingApprovals.FirstOrDefault(t => t.Attributes["Request"].ReferenceValue == requestId);
                Assert.IsNotNull(ourApproval);

                // Make sure that the request was logged, using the user account running in the current context
                pendingApprovals = client.GetApprovals(ApprovalStatus.Pending);
                ourApproval = pendingApprovals.FirstOrDefault(t => t.Attributes["Request"].ReferenceValue == requestId);
                Assert.IsNotNull(ourApproval);

                client.Approve(ourApproval, true, "Test reason for approval");

                Thread.Sleep(5000);
                ourApproval.Refresh();

                Assert.AreEqual("Approved", ourApproval.Attributes["ApprovalStatus"].StringValue);
            }
            finally
            {
                if (group?.IsPlaceHolder == false)
                {
                    client.DeleteResource(group);
                }
            }
        }

        private void AddTestUserToGroup(ResourceObject group, ResourceObject member, ConnectionMode connectionMode)
        {
            var original = UnitTestHelper.ServiceProvider.GetService<IOptions<ResourceManagementClientOptions>>();
            var options = ResourceManagementClientOptions.Clone(original.Value);
            options.Username = $"{this.standardUserCredentials.Domain}\\{this.standardUserCredentials.UserName}";
            options.Password = this.standardUserCredentials.Password;
            options.ConnectionMode = connectionMode;

            var newClient = new ResourceManagementClient(options);

            ResourceObject group2 = newClient.GetResource(group.ObjectID);
            group2.AddValue("ExplicitMember", member);
            group2.Save();
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestRejectionForCurrentUser(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject group = null;
            ResourceObject owner = null;
            ResourceObject member = null;

            string groupName = Guid.NewGuid().ToString();

            try
            {
                member = client.GetResourceByKey("Person", "AccountName", this.standardUserCredentials.UserName);
                owner = client.GetResourceByKey("Person", "AccountName", Environment.UserName);

                group = client.CreateResource("Group");
                group.SetValue("AccountName", groupName);
                group.SetValue("DisplayName", $"Unit test approval group {groupName}");
                group.SetValue("Owner", owner);
                group.SetValue("DisplayedOwner", owner);
                group.SetValue("Scope", "Global");
                group.SetValue("Type", "Security");
                group.SetValue("Domain", Environment.UserDomainName);
                group.SetValue("MembershipLocked", false);
                group.SetValue("MembershipAddWorkflow", "Owner Approval");

                client.SaveResource(group);

                UniqueIdentifier requestId = null;

                Assert.ThrowsException<AuthorizationRequiredException>(() =>
                {
                    try
                    {
                        this.AddTestUserToGroup(group, member, connectionMode);
                    }
                    catch (AuthorizationRequiredException ex)
                    {
                        if (ex.ResourceReference != null)
                        {
                            requestId = new UniqueIdentifier(ex.ResourceReference);
                        }

                        throw;
                    }
                });

                Assert.IsNotNull(requestId);
                var pendingApprovals = client.GetApprovals(ApprovalStatus.Pending);
                var ourApproval = pendingApprovals.FirstOrDefault(t => t.Attributes["Request"].ReferenceValue == requestId);
                Assert.IsNotNull(ourApproval);

                client.Approve(ourApproval, false, "Test reason for rejection");

                Thread.Sleep(5000);
                ourApproval.Refresh();

                Assert.AreEqual("Rejected", ourApproval.Attributes["ApprovalStatus"].StringValue);
            }
            finally
            {
                if (group?.IsPlaceHolder == false)
                {
                    client.DeleteResource(group);
                }
            }
        }
    }
}