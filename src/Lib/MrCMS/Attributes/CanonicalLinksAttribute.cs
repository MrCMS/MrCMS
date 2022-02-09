using System;

namespace MrCMS.Attributes
{
    public class CanonicalLinksAttribute : Attribute
    {
        public CanonicalLinksAttribute()
        {
        }

        public CanonicalLinksAttribute(string pagedDataKey = null,
            PageInfoMethod setPageInfo = PageInfoMethod.SetFromPage)
        {
            PagedDataKey = pagedDataKey;
            SetPageInfo = setPageInfo;
        }

        public string PagedDataKey { get; }

        public PageInfoMethod SetPageInfo { get; }
    }

    public enum PageInfoMethod
    {
        SetFromPage,
        SetFromViewData,
        DoNothing
    }
}