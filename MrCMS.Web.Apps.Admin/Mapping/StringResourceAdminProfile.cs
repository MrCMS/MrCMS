using AutoMapper;
using MrCMS.Entities.Resources;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class StringResourceAdminProfile : Profile
    {
        public StringResourceAdminProfile()
        {
            CreateMap<StringResource, UpdateStringResourceModel>().ReverseMap();
            CreateMap<StringResource, AddStringResourceModel>().ReverseMap()
                .MapEntityLookup(x => x.SiteId, x => x.Site)
                ;
        }
    }
}