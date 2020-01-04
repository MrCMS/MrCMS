using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lucene.Net.Index;
using MrCMS.Search;
using MrCMS.Services;

namespace MrCMS.Tasks
{
    public static class UniversalSearchActionExecutor
    {
        public static async Task PerformActions(IUniversalSearchIndexManager universalSearchIndexManager,
            ISearchConverter searchConverter, List<UniversalSearchIndexData> searchIndexDatas,
            IEventContext eventContext)
        {
            if (!searchIndexDatas.Any())
                return;

            universalSearchIndexManager.EnsureIndexExists();
            using (eventContext.Disable<UpdateUniversalSearch>())
            {
                List<UniversalSearchIndexData> toAdd =
                    searchIndexDatas.Where(data => data.Action == UniversalSearchIndexAction.Insert).ToList();
                List<UniversalSearchIndexData> toUpdate =
                    searchIndexDatas.Where(data => data.Action == UniversalSearchIndexAction.Update).ToList();
                List<UniversalSearchIndexData> toDelete =
                    searchIndexDatas.Where(data => data.Action == UniversalSearchIndexAction.Delete).ToList();
                await universalSearchIndexManager.Write(writer =>
                 {
                     foreach (UniversalSearchIndexData indexData in toAdd)
                         writer.AddDocument(searchConverter.Convert(indexData.UniversalSearchItem));
                     foreach (UniversalSearchIndexData indexData in toUpdate)
                         writer.UpdateDocument(GetTerm(indexData), searchConverter.Convert(indexData.UniversalSearchItem));
                     foreach (UniversalSearchIndexData indexData in toDelete)
                         writer.DeleteDocuments(GetTerm(indexData));
                     return Task.CompletedTask;
                 });
            }
        }

        private static Term GetTerm(UniversalSearchIndexData indexData)
        {
            return new Term(UniversalSearchFieldNames.SearchGuid, indexData.UniversalSearchItem.SearchGuid.ToString());
        }
    }
}