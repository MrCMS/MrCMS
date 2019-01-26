using System.Collections.Generic;
using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public interface IGetDocumentsByParent<T> where T : Document
    {
        IEnumerable<T> GetDocuments(T parent);
    }
}