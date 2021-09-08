using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Core.Models;
using NHibernate;
using X.PagedList;

namespace MrCMS.Web.Apps.Core.Services
{
    public class TagPageUIService : ITagPageUIService
    {
        private readonly ISession _session;

        public TagPageUIService(ISession session)
        {
            _session = session;
        }

        public IPagedList<Webpage> GetWebpages(TagPage page, TagPageSearchModel model)
        {
            TagPage tagPageAlias = null;
            var query = _session.QueryOver<Webpage>()
                .JoinAlias(webpage => webpage.TagPages, () => tagPageAlias)
                .Where(x => tagPageAlias.Id == page.Id)
                .Where(x => x.Published)
                .OrderBy(y => y.PublishOn).Desc;

            return query.Paged(model.Page, pageSize:model.PageSize);
        }

        public async Task<TagPage> GetPage(int id)
        {
            return await _session.GetAsync<TagPage>(id);
        }
    }
}

