using MrCMS.Entities.Widget;
using System.Threading;
using System.Threading.Tasks;

namespace MrCMS.Services
{
    public interface IWidgetUIService
    {
        Task<T> GetWidgetAsync<T>(int id, CancellationToken token = default) where T : Widget;
    }
}