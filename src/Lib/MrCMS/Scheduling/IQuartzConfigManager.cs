using System.Threading.Tasks;
using MrCMS.Tasks;

namespace MrCMS.Scheduling
{
    public interface IQuartzConfigManager
    {
        Task UpdateConfig(params TaskInfo[] info);
    }
}