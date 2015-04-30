using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ITagAdminService
    {
        IEnumerable<AutoCompleteResult> Search(string term);
        IEnumerable<Tag> GetTags(Document document);
    }
}