using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface ITagAdminService
    {
        Task<IEnumerable<AutoCompleteResult>> Search(string term);
        IEnumerable<Tag> GetTags(Webpage webpage);
    }
}