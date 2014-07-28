using System;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Helpers
{
    public static class PageTemplateExtensions
    {
        public static Type GetPageType(this PageTemplate template)
        {
            return template != null
                ? TypeHelper.GetTypeByName(template.PageType)
                : null;
        }

        public static Type GetUrlGeneratorType(this PageTemplate template)
        {
            return template != null
                ? TypeHelper.GetTypeByName(template.UrlGeneratorType)
                : null;
        }
    }
}