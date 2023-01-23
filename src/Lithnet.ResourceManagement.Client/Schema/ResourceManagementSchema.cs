using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// A static class that reflects the schema of the Resource Management Service
    /// </summary>
    [Obsolete("Please use the instance methods on the ResourceManagementClient object instead of this static object")]
    public static class ResourceManagementSchema
    {
        internal static ISchemaClient schemaClient;

        /// <summary>
        /// Initializes the static instance of the ResourceManagementSchema class
        /// </summary>
        static ResourceManagementSchema()
        {
            //schemaClient = new SchemaClient();
        }

        internal static void LoadSchema()
        {
            AsyncContext.Run(async () => await schemaClient.LoadSchemaAsync().ConfigureAwait(false));
        }

        /// <summary>
        /// Forces a download of the schema from the ResourceManagementService
        /// </summary>
        /// <remarks>
        /// This method should be called after making changes to ResourceObjects that form the schema
        /// </remarks>
        internal static void RefreshSchema()
        {
            AsyncContext.Run(async () => await schemaClient.RefreshSchemaAsync().ConfigureAwait(false));
        }

        /// <summary>
        /// Gets the data type of the specific attribute
        /// </summary>
        /// <param name="attributeName">The attribute name</param>
        /// <returns>An <c>AttributeType</c> value</returns>
        public static AttributeType GetAttributeType(string attributeName)
        {
            return AsyncContext.Run(async () => await schemaClient.GetAttributeTypeAsync(attributeName).ConfigureAwait(false));
        }

        /// <summary>
        /// Gets an object type definition from the schema by name
        /// </summary>
        /// <param name="name">The system name of the object type</param>
        /// <returns>A ObjectTypeDefinition with a system name that matches the 'name' parameter</returns>
        /// <exception cref="NoSuchObjectTypeException">Throw when an object type could not be found in the schema with a matching name</exception>
        public static ObjectTypeDefinition GetObjectType(string name)
        {
            return AsyncContext.Run(async () => await schemaClient.GetObjectTypeAsync(name).ConfigureAwait(false));
        }

        /// <summary>
        /// Gets a value indicating if the object type exists in the schema
        /// </summary>
        /// <param name="name">The system name of the object type</param>
        /// <returns>True if the object type exists in the schema, false if it does not</returns>
        public static bool ContainsObjectType(string name)
        {
            return AsyncContext.Run(async () => await schemaClient.ContainsObjectTypeAsync(name).ConfigureAwait(false));
        }

        /// <summary>
        /// Gets each object type definition from the schema 
        /// </summary>
        /// <returns>An enumeration of ObjectTypeDefinitions</returns>
        public static IEnumerable<ObjectTypeDefinition> GetObjectTypes()
        {
            return AsyncContext.Run(async () => await schemaClient.GetObjectTypesAsync().ToListAsync().ConfigureAwait(false));
        }

        /// <summary>
        /// Gets a value indicating whether the specific attribute is multivalued
        /// </summary>
        /// <param name="attributeName">The attribute name</param>
        /// <returns>A value indicating whether the specific attribute is multivalued</returns>
        public static bool IsAttributeMultivalued(string attributeName)
        {
            return AsyncContext.Run(async () => await schemaClient.IsAttributeMultivaluedAsync(attributeName).ConfigureAwait(false));
        }

        /// <summary>
        /// Validates that an attribute name contains only valid characters
        /// </summary>
        /// <param name="attributeName">The name of the attribute to validate</param>
        public static void ValidateAttributeName(string attributeName)
        {
            AsyncContext.Run(async () => await schemaClient.ValidateAttributeNameAsync(attributeName).ConfigureAwait(false));
        }

        /// <summary>
        /// Validates that an object type name contains only valid characters
        /// </summary>
        /// <param name="objectTypeName">The name of the object type to validate</param>
        public static void ValidateObjectTypeName(string objectTypeName)
        {
            AsyncContext.Run(async () => await schemaClient.ValidateObjectTypeNameAsync(objectTypeName).ConfigureAwait(false));
        }
    }
}