using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IDocumentMetadataService
    {
        IEnumerable<DocumentMetadata> WebpageMetadata { get; }
        List<DocumentMetadata> GetDocumentMetadatas();
        DocumentMetadata GetDocumentMetadata(IHtmlHelper helper, int id);
        Type GetTypeByName(string name);
        string GetIconClass(Document document);
        DocumentMetadata GetMetadata(Type getType);
        DocumentMetadata GetMetadata(Document document);
        int? GetMaxChildNodes(Document document);
        List<DocumentMetadata> GetValidParentTypes(Webpage webpage);
        DocumentMetadata GetMetadataByTypeName(string name);
    }
}