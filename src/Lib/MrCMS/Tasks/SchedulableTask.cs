using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public abstract class SchedulableTask 
    {
        public async Task Execute()
        {
            await OnExecute();
        }
        protected abstract Task OnExecute();
    }
}