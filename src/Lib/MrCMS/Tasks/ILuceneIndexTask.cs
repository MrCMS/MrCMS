using System.Collections.Generic;

namespace MrCMS.Tasks
{
    internal interface ILuceneIndexTask
    {
        IEnumerable<LuceneAction> GetActions();
    }
}