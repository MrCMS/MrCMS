using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using MrCMS.Search;
using MrCMS.Services;

namespace MrCMS.Tasks
{
    public class UniversalSearchActionExecutor
    {
        public static void PerformActions(IUniversalSearchIndexManager universalSearchIndexManager,
            ISearchConverter searchConverter, List<UniversalSearchIndexData> searchIndexDatas)
        {
            universalSearchIndexManager.EnsureIndexExists();
            using (EventContext.Instance.Disable<UpdateUniversalSearch>())
            {
                List<UniversalSearchIndexData> toAdd =
                    searchIndexDatas.Where(data => data.Action == UniversalSearchIndexAction.Insert).ToList();
                List<UniversalSearchIndexData> toUpdate =
                    searchIndexDatas.Where(data => data.Action == UniversalSearchIndexAction.Update).ToList();
                List<UniversalSearchIndexData> toDelete =
                    searchIndexDatas.Where(data => data.Action == UniversalSearchIndexAction.Delete).ToList();
                universalSearchIndexManager.Write(writer =>
                {
                    foreach (UniversalSearchIndexData indexData in toAdd)
                        writer.AddDocument(searchConverter.Convert(indexData.UniversalSearchItem));
                    foreach (UniversalSearchIndexData indexData in toUpdate)
                        writer.UpdateDocument(GetTerm(indexData), searchConverter.Convert(indexData.UniversalSearchItem));
                    foreach (UniversalSearchIndexData indexData in toDelete)
                        writer.DeleteDocuments(GetTerm(indexData));

                    //writer.Optimize();
                });
            }
        }

        private static Term GetTerm(UniversalSearchIndexData indexData)
        {
            return new Term(UniversalSearchFieldNames.SearchGuid, indexData.UniversalSearchItem.SearchGuid.ToString());
        }
    }
}