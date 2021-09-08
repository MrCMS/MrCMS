using System.Threading.Tasks;

namespace MrCMS.Events.Documents
{
    public interface IDocumentModifiedUser
    {
        Task<string> GetInfo();
    }
}