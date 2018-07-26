using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Admin.Models.UserEdit
{
    public abstract class UserTab : UserTabBase
    {
        public abstract string TabHtmlId { get; }
        public abstract Task RenderTabPane(IHtmlHelper<User> html, User user);
    }
}