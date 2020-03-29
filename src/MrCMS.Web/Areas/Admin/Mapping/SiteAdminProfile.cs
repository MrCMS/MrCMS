using AutoMapper;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Mapping
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