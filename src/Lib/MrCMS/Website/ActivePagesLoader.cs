using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using NHibernate;

namespace MrCMS.Website
{
    public class ActivePagesLoader : IActivePagesLoader
    {
        private readonly ISession _session;

        public ActivePagesLoader(ISession session)
        {
            _session = session;
        }

        public List<Webpage> GetActivePages(Webpage webpage)
        {
            var pages = new List<Webpage>();
            var page = webpage;
            while (page != null)
            {
                pages.Add(page);
                page = page.Parent != null ? _session.Get<Webpage>(page.Parent.Id) : null;
            }

            return pages;
        }
    }
}