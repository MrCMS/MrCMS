using MrCMS.Entities.Widget;

namespace MrCMS.Services.Widgets
{
    public abstract class GetWidgetModelBase
    {
        public abstract object GetModel(Widget widget);
    }

    public abstract class GetWidgetModelBase<T> : GetWidgetModelBase where T : Widget
    {
        public abstract object GetModel(T widget);
        public sealed override object GetModel(Widget widget)
        {
            return GetModel(widget as T);
        }
    }
}