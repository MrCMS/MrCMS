using AutoMapper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Models.WebpageEdit;
using MrCMS.Web.Apps.Core.Areas.Admin.Models;

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
                .ForMember(webpage => webpage.FrontEndAllowedRoles, expression => expression.ResolveUsing<FrontEndAllowedRoleMapper>());
            CreateMap<Webpage, WebpagePropertiesTabViewModel>().ReverseMap();
            CreateMap<Webpage, SEOTabViewModel>().ReverseMap();

            CreateMap<Redirect, RedirectViewModel>().ReverseMap();
            CreateMap<Redirect, RedirectSEOTabViewModel>().ReverseMap();
        }
    }
}