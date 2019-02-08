using System.Collections.Generic;
using AutoMapper;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Infrastructure.Models;
using MrCMS.Web.Apps.Admin.Infrastructure.Services;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Mapping
{
    public class DocumentTagsMapper : IValueResolver<IHaveTagList, Webpage, ISet<Tag>> 
    {
        private readonly IGetDocumentTagsService _getDocumentTagsService;

        public DocumentTagsMapper(IGetDocumentTagsService getDocumentTagsService)
        {
            _getDocumentTagsService = getDocumentTagsService;
        }

        public ISet<Tag> Resolve(IHaveTagList source, Webpage destination, ISet<Tag> destMember, ResolutionContext context)
        {
            return _getDocumentTagsService.GetTags(source.TagList);
        }

    }
}