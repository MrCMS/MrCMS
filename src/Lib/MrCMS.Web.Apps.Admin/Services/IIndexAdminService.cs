using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Indexing.Management;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IIndexAdminService
    {
        List<UpdateLuceneFieldBoostModel> GetBoosts(string type);
        Task SaveBoosts(List<UpdateLuceneFieldBoostModel> boosts);

        List<MrCMSIndex> GetIndexes();
        void Reindex(string typeName);

        MrCMSIndex GetUniversalSearchIndexInfo();
        void ReindexUniversalSearch();
    }
}