using AutoMapper;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class FormPropertyAdminProfile : Profile
    {
        public FormPropertyAdminProfile()
        {
            CreateMap<FormProperty, AddFormPropertyModel>()
                .ReverseMap()
                .MapEntityLookup(model => model.FormId, property => property.Form);

            CreateMap<FormProperty, UpdateFormPropertyModel>()
                .ForMember(model => model.ShowPlaceholder,
                    expression => expression.MapFrom(x => x.Unproxy() is IHavePlaceholder))
                .ForMember(model => model.Placeholder,
                    expression => expression.MapFrom(x =>
                        x.Unproxy() is IHavePlaceholder
                            ? ((IHavePlaceholder)x.Unproxy()).PlaceHolder
                            : string.Empty))
                .ReverseMap();

            CreateMap<TextBox, UpdateFormPropertyModel>()
                .ForMember(model => model.ShowPlaceholder, expression => expression.MapFrom(x => true))
                .ReverseMap();
            
            CreateMap<TextArea, UpdateFormPropertyModel>()
                .ForMember(model => model.ShowPlaceholder, expression => expression.MapFrom(x => true))
                .ReverseMap();

            CreateMap<Email, UpdateFormPropertyModel>()
                .ForMember(model => model.ShowPlaceholder, expression => expression.MapFrom(x => true))
                .ReverseMap();
        }
    }
}