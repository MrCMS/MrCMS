using System.Collections.Generic;
using MrCMS.Indexing.Management;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IIndexAdminService
    {
        List<LuceneFieldBoost> GetBoosts(string type);
        void SaveBoosts(List<LuceneFieldBoost> boosts);

        List<MrCMSIndex> GetIndexes();
        void Reindex(string typeName);
        void Optimise(string typeName);

        // TODO: universal search
        //MrCMSIndex GetUniversalSearchIndexInfo();
        //void ReindexUniversalSearch();
        //void OptimiseUniversalSearch();
    }
}