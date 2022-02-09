using System.Threading.Tasks;
using MrCMS.Entities.Widget;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IGetWidgetCachingInfo
    {
        Task<CachingInfo> Get(int id);
        Task<CachingInfo> Get(Widget widget);
    }
}