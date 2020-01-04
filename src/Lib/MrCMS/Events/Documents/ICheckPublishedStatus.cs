using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Events.Documents
{
    public interface ICheckPublishedStatus
    {
        Task Check(Webpage webpage);
    }
}