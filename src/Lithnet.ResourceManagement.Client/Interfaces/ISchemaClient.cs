using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    internal interface ISchemaClient
    {
        Regex AttributeNameValidationRegex
        {
            get;
        }

        Regex ObjectTypeNameValidationRegex
        {
            get;
        }

        Task<bool> ContainsObjectTypeAsync(string name);

        Task<AttributeType> GetAttributeTypeAsync(string attributeName);

        Task<ObjectTypeDefinition> GetObjectTypeAsync(string name);

        IAsyncEnumerable<ObjectTypeDefinition> GetObjectTypesAsync();

        Task<bool> IsAttributeMultivaluedAsync(string attributeName);

        Task LoadSchemaAsync();

        Task RefreshSchemaAsync();

        void ValidateAttributeName(string attributeName);

        void ValidateObjectTypeName(string objectTypeName);
    }
}