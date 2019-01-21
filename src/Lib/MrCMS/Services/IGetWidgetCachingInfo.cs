using MrCMS.Entities.Widget;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IGetWidgetCachingInfo
    {
        CachingInfo Get(int id);
        CachingInfo Get(Widget widget);
    }
}