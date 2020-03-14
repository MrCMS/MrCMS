using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public class GetDocumentByUrl<T> : IGetDocumentByUrl<T> where T : Document
    {
        private readonly IRepository<T> _repository;

        public GetDocumentByUrl(IRepository<T> repository)
        {
            _repository = repository;
        }

        public Task<T> GetByUrl(string url)
        {
            return _repository
                .Query()
                .FirstOrDefaultAsync(doc => doc.UrlSegment == url);
        }
    }
}