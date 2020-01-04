using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Widget;

namespace MrCMS.Services
{
    public class WidgetService : IWidgetService
    {
        private readonly IRepository<Widget> _repository;

        public WidgetService(IRepository<Widget> repository)
        {
            _repository = repository;
        }

        public async Task<T> GetWidget<T>(int id) where T : Widget
        {
            return await _repository.Load<Widget, T>(id);
        }

        public async Task SaveWidget(Widget widget)
        {
            await _repository.Update(widget);
        }

        public async Task DeleteWidget(Widget widget)
        {
            await _repository.Delete(widget);
        }

        public async Task<Widget> AddWidget(Widget widget)
        {
            await _repository.Add(widget);
            return widget;
        }
    }
}