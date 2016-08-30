using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Lithnet.ResourceManagement.Client
{
    internal class ClientConfigurationSection : ConfigurationSection
    {
        internal static ClientConfigurationSection GetConfiguration()
        {
            ClientConfigurationSection section = (ClientConfigurationSection)ConfigurationManager.GetSection("lithnetResourceManagementClient");

            if (section == null)
            {
                section = new ClientConfigurationSection();
            }

            return section;
        }

        [ConfigurationProperty("resourceManagementServiceBaseAddress", IsRequired = true, DefaultValue = "http://localhost:5725")]
        public Uri ResourceManagementServiceBaseAddress
        {
            get
            {
                return (Uri)this["resourceManagementServiceBaseAddress"];
            }
            set
            {
                this["resourceManagementServiceBaseAddress"] = value;
            }
        }

        [ConfigurationProperty("servicePrincipalName", IsRequired = false)]
        public string ServicePrincipalName
        {
            get
            {
                return (string)this["servicePrincipalName"];
            }
            set
            {
                this["servicePrincipalName"] = value;
            }
        }

        [ConfigurationProperty("forceKerberos", IsRequired = false, DefaultValue = false)]
        public bool ForceKerberos
        {
            get
            {
                return (bool)this["forceKerberos"];
            }
            set
            {
                this["forceKerberos"] = value;
            }
        }

        [ConfigurationProperty("username", IsRequired = false)]
        public string Username
        {
            get
            {
                return (string)this["username"];
            }
            set
            {
                this["username"] = value;
            }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get
            {
                return (string)this["password"];
            }
            set
            {
                this["password"] = value;
            }
        }

        [ConfigurationProperty("concurrentConnectionLimit", IsRequired = false, DefaultValue = 10000)]
        public int ConcurrentConnectionLimit
        {
            get
            {
                return (int)this["concurrentConnectionLimit"];
            }
            set
            {
                this["concurrentConnectionLimit"] = value;
            }
        }

        [ConfigurationProperty("sendTimeout", IsRequired = false, DefaultValue = 20 * 60)]
        public int SendTimeoutSeconds
        {
            get
            {
                return (int)this["sendTimeout"];
            }
            set
            {
                this["sendTimeout"] = value;
            }
        }

        [ConfigurationProperty("receiveTimeout", IsRequired = false, DefaultValue = 20 * 60)]
        public int ReceiveTimeoutSeconds
        {
            get
            {
                return (int)this["receiveTimeout"];
            }
            set
            {
                this["receiveTimeout"] = value;
            }
        }
    }
}
