using System.Collections.Generic;
using System.Linq;
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
            var webpages = new List<Webpage>();
            foreach (var item in page.Documents)
                webpages.Add(_session.Get<Webpage>(item.Id));
            return EnumerableHelper.ToPagedList(webpages.Where(x => x.Published).OrderByDescending(x => x.PublishOn),
                model.Page, pageSize: model.PageSize);
        }

        public TagPage GetPage(in int id)
        {
            return _session.Get<TagPage>(id);
        }
    }
}