using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public class WebpageTagsUpdateService : IWebpageTagsUpdateService
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IGetExistingTag _getExistingTag;

        public WebpageTagsUpdateService(IRepository<Webpage> webpageRepository, IRepository<Tag> tagRepository,
            IGetExistingTag getExistingTag)
        {
            _webpageRepository = webpageRepository;
            _tagRepository = tagRepository;
            _getExistingTag = getExistingTag;
        }

        public async Task SetTags(string taglist, int id)
        {
            var document = await _webpageRepository.Get(id);
            await SetTags(taglist, document);
        }

        public async Task SetTags(string taglist, Webpage webpage)
        {
            if (webpage == null) throw new ArgumentNullException(nameof(webpage));

            if (taglist == null)
                taglist = string.Empty;

            var tagNames =
                taglist.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(
                    x => !string.IsNullOrWhiteSpace(x)).ToList();

            await SetTags(tagNames, webpage);
        }

        public async Task SetTags(List<string> taglist, Webpage webpage)
        {
            var tagsToAdd =
                taglist.Where(
                        s => !webpage.Tags.Select(tag => tag.Name)
                            .Contains(s, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();
            var tagsToRemove =
                webpage.Tags.Where(
                    tag => !taglist.Contains(tag.Name, StringComparer.InvariantCultureIgnoreCase)).ToList();
            foreach (var item in tagsToAdd)
            {
                var tag = await _getExistingTag.GetTag(item);
                var isNew = tag == null;
                if (isNew)
                {
                    tag = new Tag {Name = item};
                    await _tagRepository.Add(tag);
                }

                if (!webpage.Tags.Contains(tag))
                    webpage.Tags.Add(tag);

                if (!tag.Webpages.Contains(webpage))
                    tag.Webpages.Add(webpage);
                await _tagRepository.Update(tag);
            }

            foreach (var tag in tagsToRemove)
            {
                webpage.Tags.Remove(tag);
                tag.Webpages.Remove(webpage);
                await _tagRepository.Update(tag);
            }
        }
    }
}