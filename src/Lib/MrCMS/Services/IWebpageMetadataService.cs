using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IWebpageMetadataService
    {
        IEnumerable<WebpageMetadata> WebpageMetadata { get; }
        List<WebpageMetadata> GetWebpageMetadata();
        WebpageMetadata GetWebpageMetadata(IHtmlHelper helper, int id);
        Type GetTypeByName(string name);
        string GetIconClass(Webpage webpage);
        WebpageMetadata GetMetadata(Type type);
        WebpageMetadata GetMetadata(Webpage webpage);
        int? GetMaxChildNodes(Webpage webpage);
        List<WebpageMetadata> GetValidParentTypes(Webpage webpage);
        WebpageMetadata GetMetadataByTypeName(string name);
    }
}