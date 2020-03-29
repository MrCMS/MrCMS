using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents;

namespace MrCMS.Web.Admin.Infrastructure.Services
{
    public class GetDocumentTagsService : IGetDocumentTagsService
    {
        private readonly IRepository<Tag> _tagRepository;

        public GetDocumentTagsService(IRepository<Tag> tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public ISet<Tag> GetTags(string tagList)
        {
            if (tagList == null)
            {
                tagList = string.Empty;
            }

            var tagNames = tagList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x));

            return tagNames.Select(GetTag).Where(x => x != null).ToHashSet();
        }

        public IList<DocumentTag> GetDocumentTags(Document document, string tagList)
        {
            var tags = GetTags(tagList);
            return tags.Select(x => new DocumentTag {Tag = x, Document = document}).ToList();
        }

        private Tag GetTag(string arg)
        {
            return
                _tagRepository.Query().ToList()
                    .Where(tag => tag.Name != null && tag.Name.Equals(arg, StringComparison.OrdinalIgnoreCase))
                    .Take(1)
                    .SingleOrDefault()
                ?? new Tag {Name = arg};
        }
    }
}