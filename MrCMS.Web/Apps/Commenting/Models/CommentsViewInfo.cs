using System.Web.Mvc;

namespace MrCMS.Web.Apps.Commenting.Models
{
    public class CommentsViewInfo
    {
        public CommentsViewInfo()
        {
            ViewData = new ViewDataDictionary();
        }
        public string View { get; set; }
        public object Model { get; set; }
        public ViewDataDictionary ViewData { get; set; }
        public bool Disabled { get; set; }
    }
}