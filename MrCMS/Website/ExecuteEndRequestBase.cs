using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Website
{
    public abstract class ExecuteEndRequestBase
    {
        public abstract void Execute(IEnumerable<object> data);
    }

    public abstract class ExecuteEndRequestBase<T1, T2> : ExecuteEndRequestBase where T1 : EndRequestTask<T2>
    {
        public abstract void Execute(IEnumerable<T2> data);

        public override sealed void Execute(IEnumerable<object> data)
        {
            Execute(data.OfType<T2>());
        }
    }
}