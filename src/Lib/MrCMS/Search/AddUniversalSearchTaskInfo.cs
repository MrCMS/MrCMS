using MrCMS.Website;

namespace MrCMS.Search
{
    [EndRequestExecutionPriority(1)]
    public class AddUniversalSearchTaskInfo : EndRequestTask<UniversalSearchIndexData>
    {
        public AddUniversalSearchTaskInfo(UniversalSearchIndexData data)
            : base(data)
        {
        }
    }
}