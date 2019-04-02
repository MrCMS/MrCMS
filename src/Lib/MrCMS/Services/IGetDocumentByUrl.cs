using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public interface IGetDocumentByUrl<out T> where T : Document
    {
        T GetByUrl(string url);
    }
}