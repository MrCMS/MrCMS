using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IDocumentService : IGetDocumentParents
    {
        void AddDocument<T>(T document) where T : Document;
        T GetDocument<T>(int id) where T : Document;
        T SaveDocument<T>(T document) where T : Document;
        IEnumerable<T> GetAllDocuments<T>() where T : Document;
        void DeleteDocument<T>(T document) where T : Document;

        T GetDocumentByUrl<T>(string url) where T : Document;
        void SetOrders(List<SortItem> items);

        void PublishNow(Webpage document);
        void Unpublish(Webpage document);
    }
}