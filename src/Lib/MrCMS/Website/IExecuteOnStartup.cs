using System.Threading;
using System.Threading.Tasks;

namespace MrCMS.Website
{
    public interface IExecuteOnStartup
    {
        Task Execute(CancellationToken cancellationToken);
        int Order { get; }
    }
}