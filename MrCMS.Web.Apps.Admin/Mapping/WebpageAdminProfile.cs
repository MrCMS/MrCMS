using AutoMapper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Models.WebpageEdit;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class WebpageAdminProfile : Profile
    {
        public WebpageAdminProfile()
        {
            CreateMap<Webpage, AddWebpageModel>().ReverseMap();
            CreateMap<Webpage, UpdateWebpageViewModel>().ReverseMap();
            CreateMap<Webpage, FormDesignTabViewModel>().ReverseMap();
            CreateMap<Webpage, FormMessageTabViewModel>().ReverseMap();
            CreateMap<Webpage, LayoutTabViewModel>()
                .ReverseMap()
                .ForMember(model => model.PageTemplate, expression => expression.ResolveUsing<PageTemplateResolver>())
                ;
            CreateMap<Webpage, PermissionsTabViewModel>().ReverseMap();
            CreateMap<Webpage, WebpagePropertiesTabViewModel>().ReverseMap();
            CreateMap<Webpage, SEOTabViewModel>().ReverseMap();
        }
    }
}