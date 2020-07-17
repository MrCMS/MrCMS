using AutoMapper;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
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