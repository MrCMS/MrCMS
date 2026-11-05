using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Web.Admin.Infrastructure.Models;
using MrCMS.Web.Admin.Infrastructure.Services;

namespace MrCMS.Web.Admin.Infrastructure.Mapping
{
    public class WebpageTagsMapper
    {
        private readonly IGetWebpageTagsService _getWebpageTagsService;

        public WebpageTagsMapper(IGetWebpageTagsService getWebpageTagsService)
        {
            _getWebpageTagsService = getWebpageTagsService;
        }

        public ISet<Tag> Resolve(IHaveTagList source)
        {
            return _getWebpageTagsService.GetTags(source.TagList);
        }
    }
}
