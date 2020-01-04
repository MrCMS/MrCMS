using System.Collections.Generic;
using MrCMS.Data;
using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public class GetBreadcrumbs : IGetBreadcrumbs
    {
        private readonly IRepository<Document> _repository;

        public GetBreadcrumbs(IRepository<Document> repository)
        {
            _repository = repository;
        }

        public IEnumerable<Document> Get(int? parent)
        {
            if (parent.HasValue)
            {
                var document = _repository.LoadSync(parent.Value);
                while (document != null)
                {
                    yield return document;
                    document = document.Parent;
                }
            }
        }

    }
}