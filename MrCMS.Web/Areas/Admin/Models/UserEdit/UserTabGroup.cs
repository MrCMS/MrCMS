using System.Collections.Generic;

namespace MrCMS.Web.Areas.Admin.Models.UserEdit
{
    public abstract class UserTabGroup : UserTabBase
    {
        protected UserTabGroup()
        {
            Children = new List<UserTabBase>();
        }

        public List<UserTabBase> Children { get; private set; }

        public void SetChildren(List<UserTabBase> children)
        {
            Children = children;
        }
    }
}