using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface ITagService
    {
        IEnumerable<AutoCompleteResult> Search(Document document, string term);
        IEnumerable<Tag> GetTags(Document document);
    }
}