using System;
using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport.DTOs;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace MrCMS.Services.ImportExport
{
    public class UpdateTagsService : IUpdateTagsService
    {
        private readonly IDocumentTagsUpdateService _documentTagsUpdateService;
        private readonly IRepository<Tag> _tagRepository;

        public UpdateTagsService(IDocumentTagsUpdateService documentTagsUpdateService)
        {
            _documentTagsUpdateService = documentTagsUpdateService;
        }

        public void SetTags(DocumentImportDTO documentDto, Webpage webpage)
        {
            var tags = documentDto.Tags;
            _documentTagsUpdateService.SetTags(tags, webpage);
        }
    }
}