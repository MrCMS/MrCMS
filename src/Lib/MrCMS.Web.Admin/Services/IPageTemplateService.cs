using System;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Services
{
    public interface IPageTemplateService
    {
        string GetLayoutName(PageTemplate template);
        string GetPageTypeName(PageTemplate template);
        Type GetUrlGeneratorType(PageTemplate template);
    }
}