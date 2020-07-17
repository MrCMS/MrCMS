using System.Collections.Generic;
using AutoMapper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Web.Admin.Models.WebpageEdit;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Mapping
{
    public class FrontEndAllowedRoleMapper : IValueResolver<PermissionsTabViewModel, Webpage, ISet<UserRole>>
    {
        private readonly IDocumentRolesAdminService _documentRolesAdminService;

        public FrontEndAllowedRoleMapper(IDocumentRolesAdminService documentRolesAdminService)
        {
            _documentRolesAdminService = documentRolesAdminService;
        }

        public ISet<UserRole> Resolve(PermissionsTabViewModel source, Webpage destination, ISet<UserRole> destMember, ResolutionContext context)
        {
            // if should not be set
            if (!source.HasCustomPermissions || source.InheritFrontEndRolesFromParent || source.PermissionType != WebpagePermissionType.RoleBased)
            {
                // return empty collection
                return new HashSet<UserRole>();
            }
            return _documentRolesAdminService.GetFrontEndRoles(source.FrontEndRoles, source.InheritFrontEndRolesFromParent);
        }
    }
}