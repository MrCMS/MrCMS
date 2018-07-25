using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace MrCMS.Web.Apps.Core.Models.Navigation
{
    public class NavigationRecord
    {
        public NavigationRecord()
        {
            Children = new List<NavigationRecord>();
        }

        public IHtmlContent Text { get; set; }

        public IHtmlContent Url { get; set; }

        public List<NavigationRecord> Children { get; set; }
    }
}