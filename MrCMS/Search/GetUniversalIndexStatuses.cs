using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing.Utils;
using MrCMS.Search.Models;

namespace MrCMS.Search
{
    public class GetUniversalIndexStatuses : IGetUniversalIndexStatuses
    {
        private readonly IUniversalSearchIndexManager _manager;
        private readonly ISearchConverter _searchConverter;

        public GetUniversalIndexStatuses(IUniversalSearchIndexManager manager, ISearchConverter searchConverter)
        {
            _manager = manager;
            _searchConverter = searchConverter;
        }

        public Dictionary<SystemEntity, UniversalSearchIndexStatus> GetStatuses(IEnumerable<SystemEntity> entities)
        {
            _manager.EnsureIndexExists();
            IndexSearcher indexSearcher = _manager.GetSearcher();

            var systemEntities = entities.ToHashSet();
            var query = GetAllStatusesQuery(systemEntities);
            TopDocs topDocs = indexSearcher.Search(query, int.MaxValue);


            var docs =
                topDocs.ScoreDocs.Select(doc => _searchConverter.Convert(indexSearcher.Doc(topDocs.ScoreDocs[0].Doc)))
                    .ToHashSet();
            return systemEntities.ToDictionary(entity => entity,
                entity =>
                {
                    var typeName = entity.GetType().FullName;
                    var doc = docs.FirstOrDefault(document => document.Id == entity.Id && document.SystemType == typeName);
                    if (doc != null)
                        return new UniversalSearchIndexStatus
                        {
                            Exists = true,
                            Guid = doc.SearchGuid.GetValueOrDefault()
                        };
                    return new UniversalSearchIndexStatus
                    {
                        Exists = false,
                        Guid = Guid.Empty
                    };
                });

            //TopDocs topDocs =
            //    indexSearcher.Search(new TermQuery(new Term(UniversalSearchFieldNames.Id, entity.Id.ToString())),
            //        int.MaxValue);
            //if (topDocs.ScoreDocs.Any())
            //{
            //    Document doc = indexSearcher.Doc(topDocs.ScoreDocs[0].Doc);
            //    searchGuid = doc.GetValue<Guid>("search-guid");
            //    exists = true;
            //}
            //return new UniversalSearchIndexStatus
            //{
            //    Exists = exists,
            //    Guid = searchGuid
            //};
        }

        private Query GetAllStatusesQuery(IEnumerable<SystemEntity> entities)
        {
            var allStatusesQuery = new BooleanQuery();
            foreach (var entity in entities)
            {
                var item = new BooleanQuery
                {
                    {new TermQuery(new Term(UniversalSearchFieldNames.Id, entity.Id.ToString())), Occur.MUST},
                    {
                        new TermQuery(new Term(UniversalSearchFieldNames.SystemType, entity.GetType().FullName)),
                        Occur.MUST
                    }
                };
                allStatusesQuery.Add(item, Occur.SHOULD);
            }
            return allStatusesQuery;
        }
    }
}