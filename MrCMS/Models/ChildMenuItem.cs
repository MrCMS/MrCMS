using MrCMS.Website;

namespace MrCMS.Models
{
    public class ChildMenuItem 
    {
        private readonly ACLOption _aclOption;
        private readonly SubMenu _subMenu;

        public ChildMenuItem(string text, string url, ACLOption aclOption = null, SubMenu subMenu = null)
        {
            _aclOption = aclOption;
            _subMenu = subMenu;
            Url = url;
            Text = text;
        }

        public string Text { get; private set; }
        public string Url { get; private set; }

        public bool CanShow
        {
            get
            {
                if (_aclOption == null) return true;
                return _aclOption.Rule == null ||
                       _aclOption.Rule.CanAccess(CurrentRequestData.CurrentUser, _aclOption.Operation);
            }
        }

        public SubMenu Children
        {
            get { return _subMenu; }
        }
    }
}