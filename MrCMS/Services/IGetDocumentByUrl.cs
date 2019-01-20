using MrCMS.Entities.Documents;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public interface IGetDocumentByUrl<out T> where T : Document
    {
        T GetByUrl(string url);
    }
}