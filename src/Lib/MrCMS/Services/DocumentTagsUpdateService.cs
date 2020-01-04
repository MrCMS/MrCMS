using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public class DocumentTagsUpdateService : IDocumentTagsUpdateService
    {
        private readonly IJoinTableRepository<DocumentTag> _documentTagRepository;
        private readonly IGetExistingTag _getExistingTag;
        private readonly IRepository<Document> _documentRepository;
        private readonly IRepository<Tag> _tagRepository;

        public DocumentTagsUpdateService(IJoinTableRepository<DocumentTag> documentTagRepository,
            IRepository<Document> documentRepository,
            IRepository<Tag> tagRepository,
            IGetExistingTag getExistingTag)
        {
            _documentTagRepository = documentTagRepository;
            _documentRepository = documentRepository;
            _tagRepository = tagRepository;
            _getExistingTag = getExistingTag;
        }

        public async Task SetTags(string taglist, int id)
        {
            var document = _documentRepository.LoadSync(id);
            await SetTags(taglist, document);
        }

        public async Task SetTags(string taglist, Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            if (taglist == null)
                taglist = string.Empty;

            var tagNames =
                taglist.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(
                    x => !string.IsNullOrWhiteSpace(x)).ToList();

            await SetTags(tagNames, document);
        }

        public async Task SetTags(List<string> taglist, Document document)
        {
            var existingTags = await _documentTagRepository.Query().Where(x => x.DocumentId == document.Id)
                .Include(x => x.Tag).ToListAsync();
            var tagsToAdd =
               taglist.Where(
                   s => !existingTags.Select(tag => tag.Tag.Name).Contains(s, StringComparer.InvariantCultureIgnoreCase))
                   .ToList();
            var tagsToRemove =
                existingTags.Where(
                    tag => !taglist.Contains(tag.Tag.Name, StringComparer.InvariantCultureIgnoreCase)).ToList();
            foreach (var item in tagsToAdd)
            {
                var tag = _getExistingTag.GetTag(item);
                var isNew = tag == null;
                if (isNew)
                {
                    tag = new Tag { Name = item };
                    await _tagRepository.Add(tag);
                }

                var documentTag = new DocumentTag
                {
                    Document = document,
                    Tag = tag
                };
                document.DocumentTags.Add(documentTag);
                tag.DocumentTags.Add(documentTag);

                await _documentTagRepository.Add(documentTag);
            }

            if (tagsToRemove.Any())
            {
                foreach (var tag in tagsToRemove)
                {
                    document.DocumentTags.Remove(tag);
                    tag.Tag.DocumentTags.Remove(tag);
                }

                await _documentTagRepository.DeleteRange(tagsToRemove);
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