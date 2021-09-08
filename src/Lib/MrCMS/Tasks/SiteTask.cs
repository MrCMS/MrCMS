using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public abstract class SiteTask 
    {
        public abstract int Priority { get; }

        public async Task Execute()
        {
            await OnExecute();
        }
        protected abstract Task OnExecute();
    }
}