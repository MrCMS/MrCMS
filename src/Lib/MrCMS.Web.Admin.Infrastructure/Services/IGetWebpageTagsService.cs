using System.Collections.Generic;
using MrCMS.Entities.Documents;

namespace MrCMS.Web.Admin.Infrastructure.Services
{
    public interface IGetWebpageTagsService
    {
        ISet<Tag> GetTags(string tagList);
    }
}