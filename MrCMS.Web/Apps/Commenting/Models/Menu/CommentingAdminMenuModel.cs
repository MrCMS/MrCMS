using System.Collections.Generic;
using MrCMS.Models;
using MrCMS.Web.Apps.Commenting.ACL;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Commenting.Models.Menu
{
    public class CommentingAdminMenuModel : IAdminMenuItem
    {
        private IDictionary<string, List<IMenuItem>> _children;
        private readonly CommentingAdminACL _commentingAdminACL = new CommentingAdminACL();
        public string Text { get { return "Comments"; } }
        public string Url { get; private set; }
        public bool CanShow
        {
            get
            {
                return _commentingAdminACL.CanAccess(CurrentRequestData.CurrentUser, CommentingAdminACL.ShowMenu);
            }
        }
        public IDictionary<string, List<IMenuItem>> Children
        {
            get { return _children = _children ?? GetChildren(); }
        }

        private IDictionary<string, List<IMenuItem>> GetChildren()
        {
            return new Dictionary<string, List<IMenuItem>>
                       {
                           {
                               "",
                               new List<IMenuItem>
                                   {
                                       new ChildMenuItem("Comments", "/Admin/Apps/Commenting/Comment",
                                                         ACLOption.Create(_commentingAdminACL,
                                                                          CommentingAdminACL.ViewComments)),
                                       new ChildMenuItem("Settings", "/Admin/Apps/Commenting/CommentingSettings",
                                                         ACLOption.Create(_commentingAdminACL,
                                                                          CommentingAdminACL.EditSettings))
                                   }
                           }
                       };
        }

        public int DisplayOrder { get { return 90; } }
    }
}