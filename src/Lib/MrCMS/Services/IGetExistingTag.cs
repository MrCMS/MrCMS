using System.Threading.Tasks;
using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public interface IGetExistingTag
    {
        Task<Tag> GetTag(string name);
    }
}