using AutoMapper;
using MrCMS.Entities.Resources;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
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