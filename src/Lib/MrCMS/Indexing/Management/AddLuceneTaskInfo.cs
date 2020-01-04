using MrCMS.DbConfiguration.Configuration;
using MrCMS.Tasks;
using MrCMS.Website;

namespace MrCMS.Indexing.Management
{
    [EndRequestExecutionPriority(1)]
    public class AddLuceneTaskInfo : EndRequestTask<QueuedTaskInfo>
    {
        public AddLuceneTaskInfo(QueuedTaskInfo data)
            : base(data)
        {
        }
    }
}