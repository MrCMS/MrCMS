using System.Collections.Generic;
using Lucene.Net.Index;
using MrCMS.Entities;
using MrCMS.Website;

namespace MrCMS.Search
{
    public class UpdateUniversalIndexExecutor : ExecuteEndRequestBase<UpdateUniversalIndex, SystemEntity>
    {
        private readonly IGetUniversalIndexStatuses _getUniversalIndexStatuses;
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;
        private readonly IUniversalSearchItemGenerator _universalSearchItemGenerator;

        public UpdateUniversalIndexExecutor(IGetUniversalIndexStatuses getUniversalIndexStatuses,
            IUniversalSearchIndexManager universalSearchIndexManager,
            IUniversalSearchItemGenerator universalSearchItemGenerator)
        {
            _getUniversalIndexStatuses = getUniversalIndexStatuses;
            _universalSearchIndexManager = universalSearchIndexManager;
            _universalSearchItemGenerator = universalSearchItemGenerator;
        }

        public override void Execute(IEnumerable<SystemEntity> data)
        {
            var statuses = _getUniversalIndexStatuses.GetStatuses(data);

            _universalSearchIndexManager.Write(writer =>
            {
                var documents = _universalSearchItemGenerator.GenerateDocuments(statuses.Keys);
                foreach (var key in statuses.Keys)
                {
                    var status = statuses[key];
                    var document = documents[key];
                    if (status.Exists)
                        writer.UpdateDocument(new Term(UniversalSearchFieldNames.SearchGuid, status.Guid.ToString()),
                            document);
                    else
                        writer.AddDocument(document);
                }
            });
        }
    }
}