using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing.Utils;
using MrCMS.Paging;
using MrCMS.Search.Models;

namespace MrCMS.Search
{
    public class SearchConverter : ISearchConverter
    {
        private static Dictionary<string, HashSet<string>> _entityTypes;

        static SearchConverter()
        {
            _entityTypes = TypeHelper.GetAllConcreteMappedClassesAssignableFrom<SystemEntity>()
                .ToDictionary(type => type.FullName,
                    type =>
                    {
                        var types = new HashSet<string>();
                        var entityType = type;
                        while (entityType != null && typeof(SystemEntity).IsAssignableFrom(entityType))
                        {
                            types.Add(entityType.FullName);
                            entityType = entityType.BaseType;
                        }
                        return types;
                    });
        }
        public Document Convert(UniversalSearchItem item)
        {
            var document = new Document();

            document.Add(new Field(UniversalSearchFieldNames.Id, item.Id.ToString(), Field.Store.YES,
                Field.Index.NOT_ANALYZED));
            string searchGuid = (item.SearchGuid).ToString();
            document.Add(new Field(UniversalSearchFieldNames.SearchGuid, searchGuid, Field.Store.YES,
                Field.Index.NOT_ANALYZED));

            string systemType = item.SystemType ?? string.Empty;
            document.Add(new Field(UniversalSearchFieldNames.SystemType, systemType, Field.Store.YES,
                Field.Index.NOT_ANALYZED));
            foreach (string entityType in _entityTypes[systemType])
            {
                document.Add(new Field(UniversalSearchFieldNames.EntityType, entityType, Field.Store.NO,
                    Field.Index.NOT_ANALYZED));
            }

            document.Add(new Field(UniversalSearchFieldNames.DisplayName, item.DisplayName ?? string.Empty, Field.Store.YES,
                Field.Index.NOT_ANALYZED) { Boost = 5 });

            document.Add(new Field(UniversalSearchFieldNames.ActionUrl, item.ActionUrl ?? string.Empty, Field.Store.YES,
                Field.Index.NOT_ANALYZED));

            document.Add(new Field(UniversalSearchFieldNames.CreatedOn,
                DateTools.DateToString(item.CreatedOn, DateTools.Resolution.SECOND), Field.Store.YES,
                Field.Index.NOT_ANALYZED));

            foreach (string searchTerm in (item.PrimarySearchTerms ?? Enumerable.Empty<string>()).Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                document.Add(new Field(UniversalSearchFieldNames.PrimarySearchTerms, searchTerm, Field.Store.NO,
                    Field.Index.ANALYZED) { Boost = 2 });
            }
            foreach (string searchTerm in (item.SecondarySearchTerms ?? Enumerable.Empty<string>()).Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                document.Add(new Field(UniversalSearchFieldNames.SecondarySearchTerms, searchTerm, Field.Store.NO,
                    Field.Index.ANALYZED) { Boost = 0.8f });
            }
            return document;
        }

        public UniversalSearchItem Convert(Document document)
        {
            var item = new UniversalSearchItem
            {
                Id = document.GetValue<int>(UniversalSearchFieldNames.Id),
                DisplayName = document.GetValue<string>(UniversalSearchFieldNames.DisplayName),
                SearchGuid = document.GetValue<Guid>(UniversalSearchFieldNames.SearchGuid),
                SystemType = document.GetValue<string>(UniversalSearchFieldNames.SystemType),
                ActionUrl = document.GetValue<string>(UniversalSearchFieldNames.ActionUrl),
            };
            return item;
        }
    }
}