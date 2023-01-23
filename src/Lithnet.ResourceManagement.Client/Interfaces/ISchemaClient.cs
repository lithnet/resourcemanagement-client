using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    internal interface ISchemaClient
    {
        Regex AttributeNameValidationRegex { get; }

        Regex ObjectTypeNameValidationRegex { get; }

        Task<bool> ContainsObjectTypeAsync(string name);

        Task<AttributeType> GetAttributeTypeAsync(string attributeName);
        Task<string> GetCorrectAttributeNameCaseAsync(string name);
        Task<string> GetCorrectObjectTypeNameCaseAsync(string name);

        Task<ObjectTypeDefinition> GetObjectTypeAsync(string name);

        IAsyncEnumerable<ObjectTypeDefinition> GetObjectTypesAsync();

        Task<bool> IsAttributeMultivaluedAsync(string attributeName);

        Task LoadSchemaAsync();

        Task RefreshSchemaAsync();

        Task ValidateAttributeNameAsync(string attributeName);

        Task ValidateObjectTypeNameAsync(string objectTypeName);
    }
}