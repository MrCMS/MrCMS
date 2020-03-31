using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Services
{
    public interface IArticleService
    {
        public Task<Article>  Get(int id);
    }
    
    public class ArticleService : IArticleService
    {
        private readonly IRepository<Article> _repository;

        public ArticleService(IRepository<Article> repository)
        {
            _repository = repository;
        }
        public async Task<Article> Get(int id)
        {
            return await _repository.Query()
                .Include(x => x.Parent)
                .Include(x => x.DocumentTags)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}