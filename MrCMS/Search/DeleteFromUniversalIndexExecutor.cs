using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Search.Models;
using MrCMS.Website;

namespace MrCMS.Search
{
    public class DeleteFromUniversalIndexExecutor : ExecuteEndRequestBase<DeleteFromUniversalIndex, SystemEntity>
    {
        private readonly IGetUniversalIndexStatuses _getUniversalIndexStatuses;
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public DeleteFromUniversalIndexExecutor(IGetUniversalIndexStatuses getUniversalIndexStatuses,
            IUniversalSearchIndexManager universalSearchIndexManager)
        {
            _getUniversalIndexStatuses = getUniversalIndexStatuses;
            _universalSearchIndexManager = universalSearchIndexManager;
        }

        public override void Execute(IEnumerable<SystemEntity> data)
        {
            var statuses = _getUniversalIndexStatuses.GetStatuses(data);

            _universalSearchIndexManager.Write(writer =>
            {
                HashSet<UniversalSearchIndexStatus> existingItems =
                    statuses.Values.Where(status => status.Exists).ToHashSet();
                foreach (UniversalSearchIndexStatus status in existingItems)
                {
                    writer.DeleteDocuments(new Term(UniversalSearchFieldNames.SearchGuid, status.Guid.ToString()));
                }
            });
        }
    }
}