using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-75)]
    public class CopyHome : ICloneSiteParts
    {
        private readonly ISession _session;

        public CopyHome(ISession session)
        {
            _session = session;
        }

        public void Clone(Site @from, Site to)
        {
            Webpage home =
                _session.QueryOver<Webpage>()
                    .Where(webpage => webpage.Site.Id == @from.Id && webpage.Parent == null)
                    .OrderBy(webpage => webpage.DisplayOrder)
                    .Asc.Take(1)
                    .SingleOrDefault();

            Webpage copy = home.GetCopyForSite(to);
            _session.Transact(session => session.Save(copy));
        }
    }
}