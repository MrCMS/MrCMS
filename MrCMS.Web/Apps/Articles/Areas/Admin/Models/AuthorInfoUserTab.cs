using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Articles.Entities;
using MrCMS.Web.Areas.Admin.Models.UserEdit;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Models
{
    public class AuthorInfoUserTab : UserTab
    {
        public override int Order
        {
            get { return 1; }
        }

        public override string Name(User user)
        {
            return "Author Info";
        }

        public override bool ShouldShow(User user)
        {
            return true;
        }

        public override Type ParentType
        {
            get { return null; }
        }

        public override string TabHtmlId
        {
            get { return "author-info"; }
        }

        public override void RenderTabPane(HtmlHelper<User> html, User user)
        {
            html.RenderAction("Show", "AuthorInfo", new {user});
        }
    }
}