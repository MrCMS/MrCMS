using System.Collections.Generic;
using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public interface IGetDocumentParents
    {
        IEnumerable<T> GetDocumentsByParent<T>(T parent) where T : Document;
    }
}