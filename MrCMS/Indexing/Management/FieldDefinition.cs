using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
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
            // TODO: refacctor FieldNames
            //IEnumerable<IFieldDefinitionInfo> fieldDefinitionInfos = MrCMSApplication.GetAll<IFieldDefinitionInfo>();
            //FieldNames = fieldDefinitionInfos.ToDictionary(info => info.GetType(), info => info.Name);
        }

        //public static Dictionary<Type, string> FieldNames { get; set; }

        public string FieldName { get; set; }
        public float Boost { get; set; }
        public Field.Store Store { get; set; }

        public static string[] GetFieldNames(params FieldDefinition[] definitions)
        {
            return definitions.Select(definition => definition.FieldName).ToArray();
        }

        public static string GetFieldName<T>() where T : IFieldDefinitionInfo
        {
            // TODO: possibly restore
            return typeof(T).Name;
            //return FieldNames.ContainsKey(typeof(T)) ? FieldNames[typeof(T)] : string.Empty;
        }

        public static string GetDisplayName(string definitionTypeName)
        {
            //var fieldName = (TypeHelper.GetTypeByName(definitionTypeName));
            //return fieldName == null ? definitionTypeName : fieldName.DisplayName;
            // TODO: refactor to allow DI
            return definitionTypeName;
        }
    }

    public abstract class FieldDefinition<T> : FieldDefinition
    {
        public abstract List<IIndexableField> GetFields(T obj);
        public abstract Dictionary<T, List<IIndexableField>> GetFields(List<T> obj);
    }

    public abstract class FieldDefinition<T1, T2> : IFieldDefinition<T1, T2>
        where T1 : IndexDefinition<T2>
        where T2 : SystemEntity
    {
        private readonly ILuceneSettingsService _luceneSettingsService;
        private readonly string _name;
        private readonly Field.Store _store;

        protected FieldDefinition(ILuceneSettingsService luceneSettingsService, string name,
            Field.Store store = Field.Store.YES)
        {
            _luceneSettingsService = luceneSettingsService;
            _name = name;
            _store = store;
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