using AutoMapper;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class LuceneFieldBoostAdminProfile : Profile
    {
        public LuceneFieldBoostAdminProfile()
        {
            CreateMap<LuceneFieldBoost, UpdateLuceneFieldBoostModel>().ReverseMap()
                .MapEntityLookup(x => x.SiteId, x => x.Site);
        }
    }
}