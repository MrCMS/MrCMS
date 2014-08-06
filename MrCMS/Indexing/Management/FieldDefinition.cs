using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Tasks;
using MrCMS.Website;

namespace MrCMS.Indexing.Management
{
    public abstract class FieldDefinition
    {
        static FieldDefinition()
        {
            IEnumerable<IFieldDefinitionInfo> fieldDefinitionInfos = MrCMSApplication.GetAll<IFieldDefinitionInfo>();
            FieldNames = fieldDefinitionInfos.ToDictionary(info => info.GetType(), info => info.Name);
        }

        public static Dictionary<Type, string> FieldNames { get; set; }

        public string FieldName { get; set; }
        public float Boost { get; set; }
        public Field.Store Store { get; set; }
        public Field.Index Index { get; set; }

        public static string[] GetFieldNames(params FieldDefinition[] definitions)
        {
            return definitions.Select(definition => definition.FieldName).ToArray();
        }

        public static string GetFieldName<T>() where T : IFieldDefinitionInfo
        {
            return FieldNames.ContainsKey(typeof(T)) ? FieldNames[typeof(T)] : string.Empty;
        }

        public static string GetDisplayName(string definitionTypeName)
        {
            var fieldName = MrCMSApplication.Get(TypeHelper.GetTypeByName(definitionTypeName)) as IFieldDefinitionInfo;
            return fieldName == null ? definitionTypeName : fieldName.DisplayName;
        }
    }

    public abstract class FieldDefinition<T> : FieldDefinition
    {
        public abstract List<AbstractField> GetFields(T obj);
        public abstract Dictionary<T, List<AbstractField>> GetFields(List<T> obj);
    }

    public interface IFieldDefinitionInfo
    {
        string Name { get; }
        string DisplayName { get; }
        string TypeName { get; }
        Field.Store Store { get; }
        Field.Index Index { get; }
        float Boost { get; }
        Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>> GetRelatedEntities();
    }

    public interface IFieldDefinition<T1, T2> : IFieldDefinitionInfo
        where T1 : IndexDefinition<T2>
        where T2 : SystemEntity
    {
        FieldDefinition<T2> GetDefinition { get; }
    }

    public abstract class FieldDefinition<T1, T2> : IFieldDefinition<T1, T2>
        where T1 : IndexDefinition<T2>
        where T2 : SystemEntity
    {
        private readonly Field.Index _index;
        private readonly ILuceneSettingsService _luceneSettingsService;
        private readonly string _name;
        private readonly Field.Store _store;

        public FieldDefinition(ILuceneSettingsService luceneSettingsService, string name,
            Field.Store store = Field.Store.YES, Field.Index index = Field.Index.ANALYZED)
        {
            _luceneSettingsService = luceneSettingsService;
            _name = name;
            _store = store;
            _index = index;
        }

        public abstract FieldDefinition<T2> GetDefinition { get; }

        public string Name
        {
            get { return _name; }
        }

        public string DisplayName
        {
            get { return GetType().Name.Replace("FieldDefinition", "").BreakUpString(); }
        }

        public Field.Store Store
        {
            get { return _store; }
        }

        public Field.Index Index
        {
            get { return _index; }
        }

        public float Boost
        {
            get { return _luceneSettingsService.GetBoost(this); }
        }

        public virtual Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>> GetRelatedEntities()
        {
            return new Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>>();
        }

        public string TypeName
        {
            get { return GetType().FullName; }
        }
    }
}