using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface ITagService
    {
        IEnumerable<AutoCompleteResult> Search(string term, int documentId);
    }
}