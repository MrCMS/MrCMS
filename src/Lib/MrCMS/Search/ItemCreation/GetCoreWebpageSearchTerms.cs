using System.Collections.Generic;
using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Search.ItemCreation
{
    public class GetCoreWebpageSearchTerms : IGetWebpageSearchTerms
    {
        private readonly IRepository<Webpage> _repository;

        private readonly IQueryableRepository<DocumentTag> _tagRepository;
        //private readonly ISession _session;

        public GetCoreWebpageSearchTerms(IRepository<Webpage> repository, IQueryableRepository<DocumentTag> tagRepository)
        {
            _repository = repository;
            _tagRepository = tagRepository;
        }

        public IEnumerable<string> GetPrimary(Webpage webpage)
        {
            return GetPrimaryTerms(webpage);
        }

        public Dictionary<Webpage, HashSet<string>> GetPrimary(HashSet<Webpage> webpages)
        {
            return webpages.ToDictionary(webpage => webpage,
                webpage => GetPrimaryTerms(webpage).ToHashSet());
        }

        public IEnumerable<string> GetSecondary(Webpage webpage)
        {
            Tag tagAlias = null;
            var documentTags = _repository.Query()
                .Where(page => page.Id == webpage.Id)
                .Select(page => tagAlias.Name).ToList();
            return GetSecondaryTerms(webpage, documentTags);
        }

        public Dictionary<Webpage, HashSet<string>> GetSecondary(HashSet<Webpage> webpages)
        {
            if (!webpages.Any())
                return new Dictionary<Webpage, HashSet<string>>();
            string typeName = webpages.First().DocumentClrType;
            Dictionary<int, IEnumerable<string>> tagInfoDictionary = _tagRepository.Query()
                .Where(documentTag => documentTag.Document.DocumentClrType == typeName)
                .Select(documentTag =>
                    new TagInfo
                    {
                        TagName = documentTag.Tag.Name,
                        WebpageId = documentTag.DocumentId
                    }
                )
                .ToList()
                .GroupBy(info => info.WebpageId)
                .ToDictionary(infos => infos.Key, infos => infos.Select(x => x.TagName));
            return webpages.ToDictionary(webpage => webpage,
                webpage =>
                    GetSecondaryTerms(webpage,
                        tagInfoDictionary.ContainsKey(webpage.Id)
                            ? tagInfoDictionary[webpage.Id]
                            : Enumerable.Empty<string>()).ToHashSet());
        }

        private IEnumerable<string> GetPrimaryTerms(Webpage webpage)
        {
            yield return webpage.Name;
        }

        private IEnumerable<string> GetSecondaryTerms(Webpage webpage, IEnumerable<string> tags)
        {
            if (!string.IsNullOrWhiteSpace(webpage.BodyContent))
                yield return webpage.BodyContent;
            yield return webpage.UrlSegment;
            foreach (string tag in tags)
            {
                yield return tag;
            }
        }

        public class TagInfo
        {
            public int WebpageId { get; set; }
            public string TagName { get; set; }
        }
    }
}