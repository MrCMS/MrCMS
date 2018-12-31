using AutoMapper;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class SiteAdminProfile : Profile
    {
        public SiteAdminProfile()
        {
            CreateMap<Site, AddSiteModel>().ReverseMap();
            CreateMap<Site, UpdateSiteModel>().ReverseMap();
        }
    }
}