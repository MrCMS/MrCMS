using AutoMapper;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class FormListOptionAdminProfile : Profile
    {
        public FormListOptionAdminProfile()
        {
            CreateMap<FormListOption, AddFormListOptionModel>()
                .ReverseMap()
                .MapEntityLookup(model => model.FormPropertyId, property => property.FormProperty);

            CreateMap<FormListOption, UpdateFormListOptionModel>()
                .ReverseMap();
        }
    }
}