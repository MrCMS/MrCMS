using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;

namespace MrCMS.Website
{
    public interface IWidgetSortedChecker
    {
        Task<bool> IsSorted(Webpage webpage, LayoutArea layoutArea);
    }

    public class WidgetSortedChecker : IWidgetSortedChecker
    {
        private readonly IRepository<PageWidgetSort> _repository;

        public WidgetSortedChecker(IRepository<PageWidgetSort> repository)
        {
            _repository = repository;
        }

        public Task<bool> IsSorted(Webpage webpage, LayoutArea layoutArea)
        {
            return _repository.Readonly().AnyAsync(x => x.WebpageId == webpage.Id && x.LayoutAreaId == layoutArea.Id);
        }
    }
}