using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Tasks;
using MrCMS.Website;

namespace MrCMS.Indexing.Management
{
    public abstract class FieldDefinition
    {
        public string FieldName { get; set; }
        public float Boost { get; set; }
        public Field.Store Store { get; set; }

        public static string[] GetFieldNames(params FieldDefinition[] definitions)
        {
            return definitions.Select(definition => definition.FieldName).ToArray();
        }

        public static string GetDefinitionDisplayName(IHtmlHelper helper, string definitionTypeName)
        {
            var type = TypeHelper.GetTypeByName(definitionTypeName);
            var info = helper.ViewContext.HttpContext.RequestServices.GetService(type) as IFieldDefinitionInfo;
            if (info == null)
                return definitionTypeName;
            return info.DisplayName;
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

        protected FieldDefinition(ILuceneSettingsService luceneSettingsService, string name,
            Field.Store store = Field.Store.YES)
        {
            _luceneSettingsService = luceneSettingsService;
            Name = name;
            Store = store;
        }

        public abstract FieldDefinition<T2> GetDefinition { get; }

        public string Name { get; }

        public string DisplayName => GetType().Name.Replace("FieldDefinition", "").BreakUpString();

        public Field.Store Store { get; }


        public float Boost => _luceneSettingsService.GetBoost(this);

        public virtual Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>> GetRelatedEntities()
        {
            return new Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>>();
        }

        public string TypeName => GetType().FullName;
    }
}