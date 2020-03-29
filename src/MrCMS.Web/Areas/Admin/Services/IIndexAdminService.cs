using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IIndexAdminService
    {
        List<UpdateLuceneFieldBoostModel> GetBoosts(string type);
        Task SaveBoosts(List<UpdateLuceneFieldBoostModel> boosts);

        List<MrCMSIndex> GetIndexes();
        Task Reindex(string typeName);

        MrCMSIndex GetUniversalSearchIndexInfo();
        Task ReindexUniversalSearch();
    }
}