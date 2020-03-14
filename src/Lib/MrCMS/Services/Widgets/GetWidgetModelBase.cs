using System.Threading.Tasks;
using MrCMS.Entities.Widget;

namespace MrCMS.Services.Widgets
{
    public abstract class GetWidgetModelBase
    {
        public abstract Task<object> GetModel(Widget widget);
    }

    public abstract class GetWidgetModelBase<T> : GetWidgetModelBase where T : Widget
    {
        public abstract Task<object> GetModel(T widget);
        public sealed override Task<object> GetModel(Widget widget)
        {
            return GetModel(widget as T);
        }
    }
}