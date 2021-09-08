using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public interface ITriggerUrls
    {
        Task Trigger(IEnumerable<string> urls);
    }
}