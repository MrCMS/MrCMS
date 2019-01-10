using MrCMS.Entities;

namespace MrCMS.Indexing.Management
{
    public interface ILuceneSettingsService
    {
        float GetBoost<T, T2>(IFieldDefinition<T, T2> fieldDefinition)
            where T : IndexDefinition<T2>
            where T2 : SystemEntity;
    }
}