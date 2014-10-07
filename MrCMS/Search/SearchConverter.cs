using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing.Utils;
using MrCMS.Search.Models;

namespace MrCMS.Search
{
    public class SearchConverter : ISearchConverter
    {
        public Document Convert(UniversalSearchItem item)
        {
            var document = new Document();

            document.Add(new Field(UniversalSearchFieldNames.Id, item.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            var searchGuid = (item.SearchGuid ?? Guid.NewGuid()).ToString();
            document.Add(new Field(UniversalSearchFieldNames.SearchGuid, searchGuid, Field.Store.YES, Field.Index.NOT_ANALYZED));

            var systemType = item.SystemType ?? string.Empty;
            document.Add(new Field(UniversalSearchFieldNames.SystemType, systemType, Field.Store.YES, Field.Index.NOT_ANALYZED));
            foreach (var entityType in GetEntityTypes(systemType))
            {
                document.Add(new Field(UniversalSearchFieldNames.EntityType, entityType, Field.Store.NO, Field.Index.NOT_ANALYZED));
            }

            document.Add(new Field(UniversalSearchFieldNames.DisplayName, item.DisplayName, Field.Store.YES, Field.Index.NOT_ANALYZED));

            document.Add(new Field(UniversalSearchFieldNames.EditUrl, item.EditUrl ?? string.Empty, Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field(UniversalSearchFieldNames.ViewUrl, item.ViewUrl ?? string.Empty, Field.Store.YES, Field.Index.NOT_ANALYZED));

            foreach (var searchTerm in item.SearchTerms.Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                document.Add(new Field(UniversalSearchFieldNames.SearchTerms, searchTerm, Field.Store.NO, Field.Index.ANALYZED));
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
                EditUrl = document.GetValue<string>(UniversalSearchFieldNames.EditUrl),
                ViewUrl = document.GetValue<string>(UniversalSearchFieldNames.ViewUrl),
            };
            return item;
        }

        private static IEnumerable<string> GetEntityTypes(string systemType)
        {
            Type entityType = TypeHelper.GetTypeByName(systemType);
            while (entityType != null && typeof(SystemEntity).IsAssignableFrom(entityType))
            {
                yield return entityType.FullName;
                entityType = entityType.BaseType;
            }
        }
    }
}