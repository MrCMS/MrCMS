using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents;

namespace MrCMS.ViewComponents
{
    public class ContentBlocksViewComponent : ViewComponent
    {
        private readonly IRepository<ContentBlock> _repository;

        public ContentBlocksViewComponent(IRepository<ContentBlock> repository)
        {
            _repository = repository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var blocks = await _repository.Query().Where(x => x.WebpageId == id).ToListAsync();

            return View(blocks);
        }
    }
}