using MrCMS.Entities.Widget;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IGetWidgetCachingInfo
    {
        CachingInfo Get(Widget widget);
    }
}