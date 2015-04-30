using MrCMS.Entities;

namespace MrCMS.Indexing.Management
{
    public interface IFieldDefinition<T1, T2> : IFieldDefinitionInfo
        where T1 : IndexDefinition<T2>
        where T2 : SystemEntity
    {
        FieldDefinition<T2> GetDefinition { get; }
    }
}