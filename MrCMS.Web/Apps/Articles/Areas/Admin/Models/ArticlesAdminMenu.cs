using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Models
{
    public class ArticlesAdminMenu : IAdminMenuItem
    {
        private SubMenu _children;
        public string Text { get { return "Articles"; } }
        public string Url { get; private set; }
        public bool CanShow { get { return true; } }

        public SubMenu Children
        {
            get
            {
                return _children ??
                       (_children = GetChildren());
            }
        }

        private SubMenu GetChildren()
        {
            return new SubMenu
            {
                {
                    "",
                    new List<ChildMenuItem>
                    {
                        new ChildMenuItem("Articles", "/Admin/Apps/Articles/Articles"),
                        new ChildMenuItem("Features", "/Admin/Apps/Articles/Features")
                    }
                }
            };
        }

        public int DisplayOrder { get { return 10; } }
    }
}