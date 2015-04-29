using MrCMS.Website;

namespace MrCMS.Installation
{
    public class InitializeIndexes : EndRequestTask<int>
    {
        public InitializeIndexes() : base(0)
        {
        }
    }
}