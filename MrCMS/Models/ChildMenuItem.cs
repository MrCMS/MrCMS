using System;
using MrCMS.ACL;
using MrCMS.Website;

namespace MrCMS.Models
{
    public class ChildMenuItem : IMenuItem
    {
        private readonly ACLOption _aclOption;

        public ChildMenuItem(string text, string url, ACLOption aclOption = null)
        {
            _aclOption = aclOption;
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
    }
}