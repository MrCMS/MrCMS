using MrCMS.Entities.People;

namespace MrCMS.Entities.Documents.Web
{
    public interface IRole
    {
        Webpage Webpage { get; set; }
        int WebpageId { get; set; }
        UserRole UserRole { get; set; }
        int UserRoleId { get; set; }
        bool? IsRecursive { get; set; }
    }
}