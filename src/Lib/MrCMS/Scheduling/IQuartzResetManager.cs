using System.Threading;
using System.Threading.Tasks;

namespace MrCMS.Scheduling
{
    public interface IQuartzResetManager
    {
        Task ResetErroredTriggers(CancellationToken cancellationToken);

    }
}