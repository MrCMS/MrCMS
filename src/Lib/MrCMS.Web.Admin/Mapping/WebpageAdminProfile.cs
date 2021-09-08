using System;
using AutoMapper;
using MrCMS.Entities.Documents.Web;
 using MrCMS.Web.Admin.Infrastructure.Mapping;
 using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Models.WebpageEdit;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Webpages;

namespace MrCMS.Web.Admin.Mapping
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
            CreateMap<Webpage, LayoutTabViewModel>().ReverseMap()
                .MapEntityLookup(x => x.PageTemplateId, x => x.PageTemplate)
                ;
            CreateMap<Webpage, PermissionsTabViewModel>()
                .ReverseMap()
                .ForMember(webpage => webpage.FrontEndAllowedRoles,
                    expression => expression.MapFrom<FrontEndAllowedRoleMapper>())
                .ForMember(webpage => webpage.PasswordAccessToken,
                    expression => expression.MapFrom((model, webpage, destMember, context) =>
                    {
                        // if it should be set
                        if (model.HasCustomPermissions &&
                            model.PermissionType == WebpagePermissionType.PasswordBased)
                        {
                            // if the password is the same
                            if (model.Password == webpage.Password)
                            {
                                // keep it the same
                                return webpage.PasswordAccessToken;
                            }
                            // otherwise get a new one
                            return Guid.NewGuid();
                        }

                        // if it's not set, return null
                        return null;
                    }))
                .ForMember(webpage => webpage.Password,
                    expression => expression.MapFrom((model, webpage) =>
                    {
                        // if it should be set
                        if (model.HasCustomPermissions &&
                            model.PermissionType == WebpagePermissionType.PasswordBased)
                            // return from model
                            return (model.Password ?? string.Empty).Trim();
                        // otherwise return null
                        return null;
                    }))

                ;
            CreateMap<Webpage, WebpagePropertiesTabViewModel>().ReverseMap();
            CreateMap<Webpage, SEOTabViewModel>().ReverseMap();

            CreateMap<Redirect, RedirectViewModel>().ReverseMap();
            CreateMap<Redirect, RedirectSEOTabViewModel>().ReverseMap();
        }
    }
}