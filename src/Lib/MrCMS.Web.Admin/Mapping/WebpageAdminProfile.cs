using System;
using Mapster;
using MrCMS.Mapping;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Models.WebpageEdit;

namespace MrCMS.Web.Admin.Mapping
{
    public class WebpageAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Webpage, AddWebpageModel>();
            config.NewConfig<AddWebpageModel, Webpage>()
                .MapEntityLookup(x => x.ParentId, x => x.Parent)
                .MapEntityLookup(x => x.PageTemplateId, x => x.PageTemplate);
            config.NewConfig<Webpage, UpdateWebpageViewModel>();
            config.NewConfig<UpdateWebpageViewModel, Webpage>();
            config.NewConfig<Webpage, LayoutTabViewModel>();
            config.NewConfig<LayoutTabViewModel, Webpage>()
                .MapEntityLookup(x => x.PageTemplateId, x => x.PageTemplate);
            config.NewConfig<Webpage, PermissionsTabViewModel>();
            config.NewConfig<PermissionsTabViewModel, Webpage>()
                .Ignore(webpage => webpage.FrontEndAllowedRoles, webpage => webpage.PasswordAccessToken, webpage => webpage.Password)
                .AfterMapping((model, webpage) =>
                {
                    var roles = MapContext.Current.CreateWithServices<FrontEndAllowedRoleMapper>().Resolve(model);
                    if (webpage.FrontEndAllowedRoles == null)
                        webpage.FrontEndAllowedRoles = roles;
                    else
                        webpage.FrontEndAllowedRoles.ReplaceContents(roles);

                    // if it should be set
                    if (model.HasCustomPermissions && model.PermissionType == WebpagePermissionType.PasswordBased)
                    {
                        // keep the token if the password is the same, otherwise get a new one
                        if (model.Password != webpage.Password)
                            webpage.PasswordAccessToken = Guid.NewGuid();
                        webpage.Password = (model.Password ?? string.Empty).Trim();
                    }
                    else
                    {
                        webpage.PasswordAccessToken = null;
                        webpage.Password = null;
                    }
                });
            config.NewConfig<Webpage, WebpagePropertiesTabViewModel>();
            config.NewConfig<WebpagePropertiesTabViewModel, Webpage>();
            config.NewConfig<Webpage, SEOTabViewModel>();
            config.NewConfig<SEOTabViewModel, Webpage>();
            config.NewConfig<Redirect, RedirectViewModel>();
            config.NewConfig<RedirectViewModel, Redirect>();
            config.NewConfig<Redirect, RedirectSEOTabViewModel>();
            config.NewConfig<RedirectSEOTabViewModel, Redirect>();
        }
    }
}