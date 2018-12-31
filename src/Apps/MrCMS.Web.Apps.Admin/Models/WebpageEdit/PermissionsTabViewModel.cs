namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class PermissionsTabViewModel
    {
        public int Id { get; set; }
        public bool InheritFrontEndRolesFromParent { get; set; }
        public string FrontEndRoles { get; set; }
    }
}