using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IDocumentService
    {
        void AddDocument<T>(T document) where T : Document;
        T GetDocument<T>(int id) where T : Document;
        T SaveDocument<T>(T document) where T : Document;
        IEnumerable<T> GetAllDocuments<T>() where T : Document;
        void DeleteDocument<T>(T document) where T : Document;

        IEnumerable<T> GetDocumentsByParent<T>(T parent) where T : Document;
        T GetDocumentByUrl<T>(string url) where T : Document;
        Layout GetDefaultLayout(Webpage currentPage);
        void SetOrders(List<SortItem> items);

        IEnumerable<Document> GetParents(int? parent);

        void PublishNow(Webpage document);
        void Unpublish(Webpage document);
    }
}