using System;

namespace MrCMS.Website.CMS
{
    public class PageData
    {
        public int Id { get; set; }
        public Type Type { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }

        public bool IsPreview { get; set; }
    }
}