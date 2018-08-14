using AutoMapper;
using MrCMS.Entities.People;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class RoleAdminProfile : Profile
    {
        public RoleAdminProfile()
        {
            CreateMap<UserRole, AddRoleModel>().ReverseMap();
            CreateMap<UserRole, UpdateRoleModel>().ReverseMap();
        }
    }
    public class LuceneFieldBoostAdminProfile : Profile
    {
        public LuceneFieldBoostAdminProfile()
        {
            CreateMap<LuceneFieldBoost, UpdateLuceneFieldBoostModel>().ReverseMap()
                .MapEntityLookup(x => x.SiteId, x => x.Site);
        }
    }
}