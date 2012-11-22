using MrCMS.Entities.People;

namespace MrCMS.Entities.Documents.Web
{
    public interface IRole
    {
        Webpage Webpage { get; set; }
        UserRole UserRole { get; set; }
        bool? IsRecursive { get; set; }
    }
}