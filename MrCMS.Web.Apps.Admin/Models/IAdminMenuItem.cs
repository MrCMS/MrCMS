using MrCMS.Models;
using MrCMS.Web.Apps.Core.Models;

namespace MrCMS.Web.Apps.Admin.Models
{
    public interface IAdminMenuItem 
    {
        string Text { get; }
        string IconClass { get; }

        string Url { get; }
        bool CanShow { get; }

        SubMenu Children { get; }
        int DisplayOrder { get; }
    }
}