using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    public interface IResourceObject
    {
        string ObjectTypeName { get; }

        OperationType ModificationType { get; }

        ObjectTypeDefinition ObjectType { get; }

        CultureInfo Locale { get; }

        IAttributeValueCollection Attributes { get; }

        bool HasPermissionHints { get; }

        bool IsReadOnly { get; }

        UniqueIdentifier ObjectID { get; }

        string DisplayName { get; }

        Dictionary<string, List<AttributeValueChange>> PendingChanges { get; }

        void UndoChanges();

        void Save(bool refresh);

        Task SaveAsync(bool refresh);

        void Save();

        Task SaveAsync();

        bool HasValue(string name);

        void Refresh();

        Task RefreshAsync();
    }
}
