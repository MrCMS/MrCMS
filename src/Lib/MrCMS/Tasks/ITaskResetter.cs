using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public interface ITaskResetter
    {
        Task ResetHungTasks();
    }
}