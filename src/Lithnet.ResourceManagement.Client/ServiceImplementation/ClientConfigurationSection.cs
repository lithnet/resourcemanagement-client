using System;
using System.Configuration;
using System.Reflection;

namespace Lithnet.ResourceManagement.Client
{
    internal class ClientConfigurationSection : ConfigurationSection
    {
        private object resourceConfigSection;

        private Type resourceConfigSectionType;

        internal static ClientConfigurationSection GetConfiguration()
        {
            ClientConfigurationSection section = (ClientConfigurationSection)ConfigurationManager.GetSection("lithnetResourceManagementClient");

            if (section == null)
            {
                object s2 = ConfigurationManager.GetSection("resourceManagementClient");

                if (s2 != null)
                {
                    section = new Client.ClientConfigurationSection(s2);
                }
                else
                {
                    section = new ClientConfigurationSection();
                }
            }

            return section;
        }

        internal ClientConfigurationSection()
        {
        }

        internal ClientConfigurationSection(object resourceConfigSection)
        {
            this.resourceConfigSection = resourceConfigSection;
            this.resourceConfigSectionType = resourceConfigSection.GetType();
        }

        [ConfigurationProperty("resourceManagementServiceBaseAddress", IsRequired = true, DefaultValue = "http://localhost:5725")]
        public Uri ResourceManagementServiceBaseAddress
        {
            get
            {
                if (this.resourceConfigSection != null)
                {
                    PropertyInfo p = this.resourceConfigSectionType.GetProperty("ResourceManagementServiceBaseAddress");
                    return (Uri)p.GetValue(this.resourceConfigSection, null);
                }
                else
                {
                    return (Uri) this["resourceManagementServiceBaseAddress"];
                }
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
                if (this.resourceConfigSection != null)
                {
                    PropertyInfo p = this.resourceConfigSectionType.GetProperty("ServicePrincipalName");
                    return (string)p.GetValue(this.resourceConfigSection, null);
                }
                else
                {
                    return (string) this["servicePrincipalName"];
                }
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
                if (this.resourceConfigSection != null)
                {
                    PropertyInfo p = this.resourceConfigSectionType.GetProperty("RequireKerberos");
                    return (bool)p.GetValue(this.resourceConfigSection, null);
                }
                else
                {
                    return (bool) this["forceKerberos"];
                }
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
                if (this.resourceConfigSection != null)
                {
                    PropertyInfo p = this.resourceConfigSectionType.GetProperty("TimeoutInMilliseconds");
                    return (int)p.GetValue(this.resourceConfigSection, null);
                }
                else
                {
                    return (int) this["sendTimeout"];
                }
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
                if (this.resourceConfigSection != null)
                {
                    PropertyInfo p = this.resourceConfigSectionType.GetProperty("TimeoutInMilliseconds");
                    return (int)p.GetValue(this.resourceConfigSection, null);
                }
                else
                {
                    return (int) this["receiveTimeout"];
                }
            }
            set
            {
                this["receiveTimeout"] = value;
            }
        }
    }
}
