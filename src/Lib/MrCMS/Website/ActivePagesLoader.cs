using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website
{
    public class ActivePagesLoader : IActivePagesLoader
    {
        private readonly IRepository<Webpage> _repository;

        public ActivePagesLoader(IRepository<Webpage> repository)
        {
            _repository = repository;
        }
        public async Task<List<Webpage>> GetActivePages(Webpage webpage)
        {
            var pages = new List<Webpage>();
            Webpage page = webpage;
            while (page != null)
            {
                pages.Add(page);
                page = page.ParentId.HasValue ? await _repository.Load(page.ParentId.Value) : null;
            }
            return pages;
        }
    }
}