using MrCMS.Entities;

namespace MrCMS.Indexing.Management
{
    public interface IFieldDefinition<T1, T2> : IFieldDefinition<T2>
        where T1 : IndexDefinition<T2>
        where T2 : SystemEntity
    {
    }
    public interface IFieldDefinition<TEntity> : IFieldDefinitionInfo
        where TEntity : SystemEntity
    {
        FieldDefinition<TEntity> GetDefinition { get; }
    }
}