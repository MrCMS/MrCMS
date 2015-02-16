using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;
using StackExchange.Profiling;

namespace MrCMS.Search.ItemCreation
{
    public class GetCoreWebpageSearchTerms : IGetWebpageSearchTerms
    {
        private readonly IStatelessSession _statelessSession;

        public GetCoreWebpageSearchTerms(IStatelessSession statelessSession)
        {
            _statelessSession = statelessSession;
        }

        public IEnumerable<string> Get(Webpage webpage)
        {
            HashSet<string> documentTags =
                _statelessSession.Query<Tag>()
                    .Where(x => x.Documents.Contains(webpage))
                    .Select(tag => tag.Name)
                    .ToHashSet();
            return GetTerms(webpage, documentTags);
        }

        public Dictionary<Webpage, HashSet<string>> Get(HashSet<Webpage> webpages)
        {
            using (MiniProfiler.Current.Step("Get all tags"))
            {
                if (!webpages.Any())
                    return new Dictionary<Webpage, HashSet<string>>();
                Tag tagAlias = null;
                TagInfo tagInfo = null;
                string typeName = webpages.First().DocumentType;
                Dictionary<int, IEnumerable<string>> tagInfoDictionary = _statelessSession.QueryOver<Webpage>()
                    .Where(webpage => webpage.DocumentType == typeName)
                    .JoinAlias(webpage => webpage.Tags, () => tagAlias)
                    .SelectList(builder =>
                        builder.Select(webpage => webpage.Id).WithAlias(() => tagInfo.WebpageId)
                            .Select(() => tagAlias.Name).WithAlias(() => tagInfo.TagName))
                    .TransformUsing(Transformers.AliasToBean<TagInfo>()).List<TagInfo>()
                    .GroupBy(info => info.WebpageId)
                    .ToDictionary(infos => infos.Key, infos => infos.Select(x => x.TagName));

                return webpages.ToDictionary(webpage => webpage,
                    webpage =>
                        GetTerms(webpage,
                            tagInfoDictionary.ContainsKey(webpage.Id)
                                ? tagInfoDictionary[webpage.Id]
                                : Enumerable.Empty<string>()).ToHashSet());
            }
        }

        private IEnumerable<string> GetTerms(Webpage webpage, IEnumerable<string> tags)
        {
            yield return webpage.Name;
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