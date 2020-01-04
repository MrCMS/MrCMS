using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    internal interface ILuceneIndexTask
    {
        Task<IEnumerable<LuceneAction>> GetActions(CancellationToken token = default);
    }
}