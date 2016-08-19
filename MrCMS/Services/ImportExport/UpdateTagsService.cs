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
        private readonly IRepository<Tag> _tagRepository;

        public UpdateTagsService(IRepository<Tag> tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public void SetTags(DocumentImportDTO documentDto, Webpage webpage)
        {
            var tagsToAdd =
                documentDto.Tags.Where(
                    s => !webpage.Tags.Select(tag => tag.Name).Contains(s, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();
            var tagsToRemove =
                webpage.Tags.Where(
                    tag => !documentDto.Tags.Contains(tag.Name, StringComparer.InvariantCultureIgnoreCase)).ToList();
            foreach (var item in tagsToAdd)
            {
                var tag = GetExistingTag(item);
                var isNew = tag == null;
                if (isNew)
                {
                    tag = new Tag { Name = item };
                    _tagRepository.Add(tag);
                }
                if (!webpage.Tags.Contains(tag))
                    webpage.Tags.Add(tag);

                if (!tag.Documents.Contains(webpage))
                    tag.Documents.Add(webpage);
                _tagRepository.Update(tag);
            }

            foreach (var tag in tagsToRemove)
            {
                webpage.Tags.Remove(tag);
                tag.Documents.Remove(webpage);
                _tagRepository.Update(tag);
            }
        }

        private Tag GetExistingTag(string item)
        {
            return
                _tagRepository.Query().ToList()
                    .Where(tag => tag.Name != null && tag.Name.Equals(item, StringComparison.OrdinalIgnoreCase))
                    .Take(1)
                    .SingleOrDefault();
        }
    }
}