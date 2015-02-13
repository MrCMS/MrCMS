using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Entities;

namespace MrCMS.Search
{
    public interface IUniversalSearchItemGenerator
    {
        bool CanGenerate(SystemEntity entity);
        Document GenerateDocument(SystemEntity entity);
        IEnumerable<Document> GetAllItems();
    }
}