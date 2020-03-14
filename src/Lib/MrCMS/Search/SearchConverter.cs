using Lucene.Net.Documents;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing.Utils;
using MrCMS.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var searchGuid = item.SearchGuid.ToString();
            var systemType = item.SystemType ?? string.Empty;
            FieldType boostableStringType = new FieldType(StringField.TYPE_STORED) { OmitNorms = false, IsStored = true };
            var document = new Document
            {
                new StringField(UniversalSearchFieldNames.Id, item.Id.ToString(), Field.Store.YES),
                new StringField(UniversalSearchFieldNames.SearchGuid, searchGuid, Field.Store.YES),
                new StringField(UniversalSearchFieldNames.SystemType, systemType, Field.Store.YES),
                new Field(UniversalSearchFieldNames.DisplayName, item.DisplayName ?? string.Empty, boostableStringType)
                {
                    Boost = 5
                },
                new StringField(UniversalSearchFieldNames.ActionUrl, item.ActionUrl ?? string.Empty, Field.Store.YES),
                new StringField(UniversalSearchFieldNames.CreatedOn,
                    DateTools.DateToString(item.CreatedOn, DateTools.Resolution.SECOND), Field.Store.YES)
            };


            //Start with a copy of the standard field type


            if (_entityTypes.ContainsKey(systemType))
            {
                foreach (var entityType in _entityTypes[systemType])
                {
                    document.Add(new StringField(UniversalSearchFieldNames.EntityType, entityType, Field.Store.NO));
                }
            }


            FieldType termStringType = new FieldType(StringField.TYPE_STORED) { OmitNorms = false, IsStored = true };
            foreach (var searchTerm in (item.PrimarySearchTerms ?? Enumerable.Empty<string>()).Where(s =>
                !string.IsNullOrWhiteSpace(s)))
            {
                document.Add(new Field(UniversalSearchFieldNames.PrimarySearchTerms, searchTerm, termStringType) { Boost = 2 });
            }

            foreach (var searchTerm in (item.SecondarySearchTerms ?? Enumerable.Empty<string>()).Where(s =>
                !string.IsNullOrWhiteSpace(s)))
            {
                document.Add(new Field(UniversalSearchFieldNames.SecondarySearchTerms, searchTerm, termStringType) { Boost = 0.8f });
            }

            return document;
        }

        public UniversalSearchItem Convert(Document document)
        {
            return new UniversalSearchItem
            {
                Id = document.GetValue<int>(UniversalSearchFieldNames.Id),
                DisplayName = document.GetValue<string>(UniversalSearchFieldNames.DisplayName),
                SearchGuid = document.GetValue<Guid>(UniversalSearchFieldNames.SearchGuid),
                SystemType = document.GetValue<string>(UniversalSearchFieldNames.SystemType),
                ActionUrl = document.GetValue<string>(UniversalSearchFieldNames.ActionUrl)
            };
        }
    }
}