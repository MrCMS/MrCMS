using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate;
using NHibernate.Mapping;
using NHibernate.Transform;

namespace MrCMS.Indexing.Definitions
{
    public class UrlSegmentFieldDefinition : StringFieldDefinition<AdminWebpageIndexDefinition, Webpage>
    {
        private readonly IStatelessSession _statelessSession;

        public UrlSegmentFieldDefinition(ILuceneSettingsService luceneSettingsService, IStatelessSession statelessSession)
            : base(luceneSettingsService, "urlsegment")
        {
            _statelessSession = statelessSession;
        }

        protected override IEnumerable<string> GetValues(Webpage obj)
        {
            yield return obj.LiveUrlSegment;
            foreach (var urlHistory in obj.Urls)
            {
                yield return urlHistory.UrlSegment;
            }
        }

        public class UrlHistoryMap
        {
            public int WebpageId { get; set; }
            public string Url { get; set; }

            public UrlHistoryData ToData()
            {
                return new UrlHistoryData
                {
                    Url = Url,
                    WebpageId = WebpageId
                };
            }
        }
        public struct UrlHistoryData
        {
            public int WebpageId { get; set; }
            public string Url { get; set; }
        }
        protected override Dictionary<Webpage, IEnumerable<string>> GetValues(List<Webpage> objs)
        {
            UrlHistoryMap map = null;
            var urlHistoryDatas = _statelessSession.QueryOver<UrlHistory>().SelectList(builder => 
                builder.Select(history => history.Webpage.Id).WithAlias(() => map.WebpageId).Select(history => history.UrlSegment).WithAlias(() => map.Url))
                .TransformUsing(Transformers.AliasToBean<UrlHistoryMap>()).List<UrlHistoryMap>().Select(history => history.ToData()).ToHashSet();

            var dictionary = urlHistoryDatas.GroupBy(data => data.WebpageId)
                .ToDictionary(datas => datas.Key, datas => datas.Select(data => data.Url).ToHashSet());

            return objs.ToDictionary(webpage => webpage,
                webpage => dictionary.ContainsKey(webpage.Id) ? dictionary[webpage.Id] : Enumerable.Empty<string>());
        }

        public override Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>> GetRelatedEntities()
        {
            return new Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>>
                   {
                       {
                           typeof (UrlHistory),
                           entity =>
                           {
                               if (entity is UrlHistory)
                               {
                                   return new List<LuceneAction>
                                          {
                                              new LuceneAction
                                              {
                                                  Entity = (entity as UrlHistory).Webpage,
                                                  Operation = LuceneOperation.Update,
                                                  IndexDefinition = IndexingHelper.Get<AdminWebpageIndexDefinition>()
                                              }
                                          };
                               }
                               return new List<LuceneAction>();
                           }
                       }
                   };
        }
    }
}