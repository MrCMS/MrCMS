using System;
using System.Collections.Generic;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Core.Models.Search;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IIndexAdminService
    {
        List<LuceneFieldBoost> GetBoosts(string type);
        void SaveBoosts(List<LuceneFieldBoost> boosts); 
    }
}