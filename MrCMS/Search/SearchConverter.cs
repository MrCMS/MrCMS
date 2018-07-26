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
        private static readonly Dictionary<string, HashSet<string>> _entityTypes;

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

            document.Add(new StringField(UniversalSearchFieldNames.Id, item.Id.ToString(), Field.Store.YES));
            var searchGuid = item.SearchGuid.ToString();
            document.Add(new StringField(UniversalSearchFieldNames.SearchGuid, searchGuid, Field.Store.YES));

            var systemType = item.SystemType ?? string.Empty;
            document.Add(new StringField(UniversalSearchFieldNames.SystemType, systemType, Field.Store.YES));
            foreach (var entityType in _entityTypes[systemType])
                document.Add(new StringField(UniversalSearchFieldNames.EntityType, entityType, Field.Store.NO));

            document.Add(new StringField(UniversalSearchFieldNames.DisplayName, item.DisplayName ?? string.Empty,
                Field.Store.YES) {Boost = 5});

            document.Add(new StringField(UniversalSearchFieldNames.ActionUrl, item.ActionUrl ?? string.Empty, Field.Store.YES));

            document.Add(new StringField(UniversalSearchFieldNames.CreatedOn,
                DateTools.DateToString(item.CreatedOn, DateTools.Resolution.SECOND), Field.Store.YES));

            foreach (var searchTerm in (item.PrimarySearchTerms ?? Enumerable.Empty<string>()).Where(s =>
                !string.IsNullOrWhiteSpace(s)))
                document.Add(new StringField(UniversalSearchFieldNames.PrimarySearchTerms, searchTerm, Field.Store.NO) {Boost = 2});
            foreach (var searchTerm in (item.SecondarySearchTerms ?? Enumerable.Empty<string>()).Where(s =>
                !string.IsNullOrWhiteSpace(s)))
                document.Add(new StringField(UniversalSearchFieldNames.SecondarySearchTerms, searchTerm, Field.Store.NO) {Boost = 0.8f});
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
                ActionUrl = document.GetValue<string>(UniversalSearchFieldNames.ActionUrl)
            };
            return item;
        }
    }
}