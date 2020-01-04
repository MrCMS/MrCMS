using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MrCMS.Website
{
    public abstract class ExecuteEndRequestBase
    {
        public abstract Task Execute(IEnumerable<object> data, CancellationToken token);
    }

    public abstract class ExecuteEndRequestBase<T1, T2> : ExecuteEndRequestBase where T1 : EndRequestTask<T2>
    {
        public abstract Task Execute(IEnumerable<T2> data, CancellationToken token);

        public sealed override Task Execute(IEnumerable<object> data, CancellationToken token)
        {
            return Execute(data.OfType<T2>(), token);
        }
    }
}