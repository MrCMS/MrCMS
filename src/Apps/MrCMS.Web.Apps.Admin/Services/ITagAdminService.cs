using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ITagAdminService
    {
        IEnumerable<AutoCompleteResult> Search(string term);
        IEnumerable<Tag> GetTags(Document document);
    }
}