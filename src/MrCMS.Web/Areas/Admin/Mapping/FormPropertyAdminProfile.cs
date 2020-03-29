using AutoMapper;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Mapping
{
    public class FormPropertyAdminProfile : Profile
    {
        public FormPropertyAdminProfile()
        {
            CreateMap<FormProperty, AddFormPropertyModel>()
                .ReverseMap() ;

            CreateMap<FormProperty, UpdateFormPropertyModel>()
                .ForMember(model => model.ShowPlaceholder,
                    expression => expression.MapFrom(x => x is IHavePlaceholder))
                .ForMember(model => model.Placeholder,
                    expression => expression.MapFrom(x =>
                        x is IHavePlaceholder
                            ? ((IHavePlaceholder)x).PlaceHolder
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