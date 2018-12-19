using AutoMapper;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Models.WebpageEdit;
using MrCMS.Web.Apps.Core.Areas.Admin.Models;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Webpages;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class WebpageAdminProfile : Profile
    {
        public WebpageAdminProfile()
        {
            CreateMap<Webpage, AddWebpageModel>().ReverseMap()
                .MapEntityLookup(x => x.ParentId, x => x.Parent)
                .MapEntityLookup(x => x.PageTemplateId, x => x.PageTemplate)
                ;
            CreateMap<Webpage, UpdateWebpageViewModel>().ReverseMap();
            CreateMap<Webpage, FormDesignTabViewModel>().ReverseMap();
            CreateMap<Webpage, FormMessageTabViewModel>().ReverseMap();
            CreateMap<Webpage, LayoutTabViewModel>().ReverseMap()
                .MapEntityLookup(x => x.PageTemplateId, x => x.PageTemplate)
                ;
            CreateMap<Webpage, PermissionsTabViewModel>().ReverseMap()
                .ForMember(webpage => webpage.FrontEndAllowedRoles, expression => expression.MapFrom<FrontEndAllowedRoleMapper>());
            CreateMap<Webpage, WebpagePropertiesTabViewModel>().ReverseMap();
            CreateMap<Webpage, SEOTabViewModel>().ReverseMap();

            CreateMap<Redirect, RedirectViewModel>().ReverseMap();
            CreateMap<Redirect, RedirectSEOTabViewModel>().ReverseMap();
        }
    }
}