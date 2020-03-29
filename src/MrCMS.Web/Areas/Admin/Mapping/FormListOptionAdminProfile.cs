using AutoMapper;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Mapping
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