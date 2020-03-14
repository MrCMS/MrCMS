using System.Threading.Tasks;
using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public interface IGetDocumentByUrl<T> where T : Document
    {
        Task<T> GetByUrl(string url);
    }
}