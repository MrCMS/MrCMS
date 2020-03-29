using AutoMapper;
using MrCMS.Entities.Resources;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Mapping
{
    public class StringResourceAdminProfile : Profile
    {
        public StringResourceAdminProfile()
        {
            CreateMap<StringResource, UpdateStringResourceModel>().ReverseMap();
            CreateMap<StringResource, AddStringResourceModel>().ReverseMap()
                ;
        }
    }
}