using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public interface IGetDocumentByUrl<out T> where T : Document
    {
        T GetByUrl(string url);
    }

    public class GetDocumentByUrl<T> : IGetDocumentByUrl<T> where T : Document
    {
        private readonly IRepository<T> _repository;

        public GetDocumentByUrl(IRepository<T> repository)
        {
            _repository = repository;
        }

        public T GetByUrl(string url)
        {
            var firstOrDefault = _repository
                .Query()
                .FirstOrDefault(doc => doc.UrlSegment == url);
            return firstOrDefault;
        }
    }
}