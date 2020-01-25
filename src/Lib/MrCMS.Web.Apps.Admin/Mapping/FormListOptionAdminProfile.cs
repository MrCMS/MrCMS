using AutoMapper;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class FormListOptionAdminProfile : Profile
    {
        public FormListOptionAdminProfile()
        {
            CreateMap<FormListOption, AddFormListOptionModel>()
                .ReverseMap();

            CreateMap<FormListOption, UpdateFormListOptionModel>()
                .ReverseMap();
        }
    }
}