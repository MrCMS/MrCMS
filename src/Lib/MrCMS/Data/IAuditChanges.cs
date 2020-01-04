using System.Threading.Tasks;

namespace MrCMS.Data
{
    public interface IAuditChanges
    {
        Task Audit(ContextChangeData data);
    }
}