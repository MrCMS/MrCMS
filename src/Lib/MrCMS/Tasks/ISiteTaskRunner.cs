using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    
    public interface ISiteTaskRunner
    {
        Task ExecuteTask(string typeName);
    }
}