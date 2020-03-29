using System.Collections.Generic;
using AutoMapper;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Infrastructure.Models;
using MrCMS.Web.Admin.Infrastructure.Services;

namespace MrCMS.Web.Admin.Infrastructure.Mapping
{
    public class DocumentTagsMapper : IValueResolver<IHaveTagList, Webpage, IList<DocumentTag>> 
    {
        private readonly IGetDocumentTagsService _getDocumentTagsService;

        public DocumentTagsMapper(IGetDocumentTagsService getDocumentTagsService)
        {
            _getDocumentTagsService = getDocumentTagsService;
        }

        public IList<DocumentTag> Resolve(IHaveTagList source, Webpage destination, IList<DocumentTag> destMember, ResolutionContext context)
        {
            return _getDocumentTagsService.GetDocumentTags(destination, source.TagList);
        }
    }
}