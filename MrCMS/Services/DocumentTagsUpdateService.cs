using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class DocumentTagsUpdateService : IDocumentTagsUpdateService
    {
        private readonly IRepository<Document> _documentRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IGetExistingTag _getExistingTag;

        public DocumentTagsUpdateService(IRepository<Document> documentRepository, IRepository<Tag> tagRepository, IGetExistingTag getExistingTag)
        {
            _documentRepository = documentRepository;
            _tagRepository = tagRepository;
            _getExistingTag = getExistingTag;
        }

        public void SetTags(string taglist, int id)
        {
            var document = _documentRepository.Get(id);
            SetTags(taglist, document);
        }

        public void SetTags(string taglist, Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            if (taglist == null)
                taglist = string.Empty;

            var tagNames =
                taglist.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(
                    x => !string.IsNullOrWhiteSpace(x)).ToList();

            SetTags(tagNames, document);
        }

        public void SetTags(List<string> taglist, Document document)
        {
            var tagsToAdd =
               taglist.Where(
                   s => !document.Tags.Select(tag => tag.Name).Contains(s, StringComparer.InvariantCultureIgnoreCase))
                   .ToList();
            var tagsToRemove =
                document.Tags.Where(
                    tag => !taglist.Contains(tag.Name, StringComparer.InvariantCultureIgnoreCase)).ToList();
            foreach (var item in tagsToAdd)
            {
                var tag = _getExistingTag.GetTag(item);
                var isNew = tag == null;
                if (isNew)
                {
                    tag = new Tag { Name = item };
                    _tagRepository.Add(tag);
                }
                if (!document.Tags.Contains(tag))
                    document.Tags.Add(tag);

                if (!tag.Documents.Contains(document))
                    tag.Documents.Add(document);
                _tagRepository.Update(tag);
            }

            foreach (var tag in tagsToRemove)
            {
                document.Tags.Remove(tag);
                tag.Documents.Remove(document);
                _tagRepository.Update(tag);
            }
        }
    }

    public interface IGetExistingTag
    {
        Tag GetTag(string name);
    }

    public class GetExistingTag : IGetExistingTag
    {
        private readonly IRepository<Tag> _tagRepository;

        public GetExistingTag(IRepository<Tag> tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public Tag GetTag(string name)
        {
            return
               _tagRepository.Query().ToList()
                   .Where(tag => tag.Name != null && tag.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                   .Take(1)
                   .SingleOrDefault();
        }
    }
}