using AutoMapper;
using MrCMS.Indexing.Management;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
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