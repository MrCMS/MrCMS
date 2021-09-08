using System;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.CMS
{
    public class PageData
    {
        public int Id { get; set; }
        public Type Type { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }

        public PageDataState DisplayState { get; set; }

        public Webpage Webpage { get; set; }
    }

    public enum PageDataState
    {
        Unpublished,
        Preview,
        Display
    }
}