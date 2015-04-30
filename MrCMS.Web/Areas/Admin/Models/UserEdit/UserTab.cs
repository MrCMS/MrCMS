using System.Web.Mvc;
using MrCMS.Entities.People;

namespace MrCMS.Web.Areas.Admin.Models.UserEdit
{
    public abstract class UserTab : UserTabBase
    {
        public abstract string TabHtmlId { get; }
        public abstract void RenderTabPane(HtmlHelper<User> html, User user);
    }
}