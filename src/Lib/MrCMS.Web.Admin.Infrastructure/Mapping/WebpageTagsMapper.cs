using System.Collections.Generic;
using AutoMapper;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Infrastructure.Models;
using MrCMS.Web.Admin.Infrastructure.Services;

namespace MrCMS.Web.Admin.Infrastructure.Mapping
{
    public class WebpageTagsMapper : IValueResolver<IHaveTagList, Webpage, ISet<Tag>> 
    {
        private readonly IGetWebpageTagsService _getWebpageTagsService;

        public WebpageTagsMapper(IGetWebpageTagsService getWebpageTagsService)
        {
            _getWebpageTagsService = getWebpageTagsService;
        }

        public ISet<Tag> Resolve(IHaveTagList source, Webpage destination, ISet<Tag> destMember, ResolutionContext context)
        {
            return _getWebpageTagsService.GetTags(source.TagList);
        }

    }
}