using MrCMS.Website;

namespace MrCMS.Services
{
    public class ApplicationRestart : EndRequestTask<int>
    {
        public ApplicationRestart()
            : base(0)
        {
        }
    }
}