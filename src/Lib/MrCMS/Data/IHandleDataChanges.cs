using System.Threading.Tasks;

namespace MrCMS.Data
{
    public interface IHandleDataChanges
    {
        Task HandleChanges(ContextChangeData data);
    }
}