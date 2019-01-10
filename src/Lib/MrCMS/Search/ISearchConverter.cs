using Lucene.Net.Documents;
using MrCMS.Search.Models;

namespace MrCMS.Search
{
    public interface ISearchConverter
    {
        Document Convert(UniversalSearchItem item);
        UniversalSearchItem Convert(Document document);
    }
}