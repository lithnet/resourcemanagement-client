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
            object section = ConfigurationManager.GetSection("lithnetResourceManagementClient");

            if (section != null)
            {
                if (section is ClientConfigurationSection configurationSection)
                {
                    return configurationSection;
                }

                return new ClientConfigurationSection(section);
            }

            object s2 = ConfigurationManager.GetSection("resourceManagementClient");

            if (s2 != null)
            {
                return new ClientConfigurationSection(s2);
            }

            return new ClientConfigurationSection();
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
                PropertyInfo p = this.GetLinkedPropertyOrNull(nameof(this.ResourceManagementServiceBaseAddress));

                if (p == null)
                {
                    return (Uri)this["resourceManagementServiceBaseAddress"];
                }

                return (Uri)p.GetValue(this.resourceConfigSection, null);
            }
            set => this["resourceManagementServiceBaseAddress"] = value;
        }

        [ConfigurationProperty("servicePrincipalName", IsRequired = false)]
        public string ServicePrincipalName
        {
            get
            {
                PropertyInfo p = this.GetLinkedPropertyOrNull(nameof(this.ServicePrincipalName));

                if (p == null)
                {
                    return (string)this["servicePrincipalName"];
                }

                return (string)p.GetValue(this.resourceConfigSection, null);
            }
            set => this["servicePrincipalName"] = value;
        }

        [ConfigurationProperty("forceKerberos", IsRequired = false, DefaultValue = false)]
        public bool ForceKerberos
        {
            get
            {
                PropertyInfo p = this.GetLinkedPropertyOrNull(nameof(this.ForceKerberos), "RequireKerberos");

                if (p == null)
                {
                    return (bool)this["forceKerberos"];
                }

                return (bool)p.GetValue(this.resourceConfigSection, null);
            }
            set => this["forceKerberos"] = value;
        }

        [ConfigurationProperty("username", IsRequired = false)]
        public string Username
        {
            get
            {
                PropertyInfo p = this.GetLinkedPropertyOrNull(nameof(this.Username));

                if (p == null)
                {
                    return (string)this["username"];
                }

                return (string)p.GetValue(this.resourceConfigSection, null);
            }
            set => this["username"] = value;
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get
            {
                PropertyInfo p = this.GetLinkedPropertyOrNull(nameof(this.Password));

                if (p == null)
                {
                    return (string)this["password"];
                }

                return (string)p.GetValue(this.resourceConfigSection, null);
            }
            set => this["password"] = value;
        }

        [ConfigurationProperty("concurrentConnectionLimit", IsRequired = false, DefaultValue = 10000)]
        public int ConcurrentConnectionLimit
        {
            get
            {
                PropertyInfo p = this.GetLinkedPropertyOrNull(nameof(this.ConcurrentConnectionLimit));

                if (p == null)
                {
                    return (int)this["concurrentConnectionLimit"];
                }

                return (int)p.GetValue(this.resourceConfigSection, null);
            }
            set => this["concurrentConnectionLimit"] = value;
        }

        [ConfigurationProperty("sendTimeout", IsRequired = false, DefaultValue = 20 * 60)]
        public int SendTimeoutSeconds
        {
            get
            {
                PropertyInfo p = this.GetLinkedPropertyOrNull(nameof(this.ReceiveTimeoutSeconds), "TimeoutInMilliseconds");

                if (p == null)
                {
                    return (int)this["sendTimeout"];
                }

                int value = (int)p.GetValue(this.resourceConfigSection, null);

                if (p.Name == nameof(this.ReceiveTimeoutSeconds))
                {
                    return value;
                }
                else
                {
                    return value / 1000; // Value came from MS section and requires conversion from milliseconds
                }
            }
            set => this["sendTimeout"] = value;
        }

        [ConfigurationProperty("receiveTimeout", IsRequired = false, DefaultValue = 20 * 60)]
        public int ReceiveTimeoutSeconds
        {
            get
            {
                PropertyInfo p = this.GetLinkedPropertyOrNull(nameof(this.ReceiveTimeoutSeconds), "TimeoutInMilliseconds");

                if (p == null)
                {
                    return (int)this["receiveTimeout"];
                }

                int value = (int)p.GetValue(this.resourceConfigSection, null);

                if (p.Name == nameof(this.ReceiveTimeoutSeconds))
                {
                    return value;
                }
                else
                {
                    return value / 1000; // Value came from MS section and requires conversion from milliseconds
                }
            }
            set => this["receiveTimeout"] = value;
        }

        private PropertyInfo GetLinkedPropertyOrNull(params string[] propertyNames)
        {
            if (this.resourceConfigSection == null)
            {
                return null;
            }

            foreach (string propertyName in propertyNames)
            {
                PropertyInfo p = this.resourceConfigSectionType.GetProperty(propertyName);

                if (p != null)
                {
                    return p;
                }
            }

            return null;
        }
    }
}
