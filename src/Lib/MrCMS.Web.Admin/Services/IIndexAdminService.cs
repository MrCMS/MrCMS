using System.Collections.Generic;
using MrCMS.Indexing.Management;
using MrCMS.Models;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IIndexAdminService
    {
        List<UpdateLuceneFieldBoostModel> GetBoosts(string type);
        void SaveBoosts(List<UpdateLuceneFieldBoostModel> boosts);

        List<MrCMSIndex> GetIndexes();
        void Reindex(string typeName);

        MrCMSIndex GetUniversalSearchIndexInfo();
        void ReindexUniversalSearch();
    }
}