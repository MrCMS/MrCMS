using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-65)]
    public class UpdateSiteSettings : ICloneSiteParts
    {
        private readonly ISession _session;

        public UpdateSiteSettings(ISession session)
        {
            _session = session;
        }

        public void Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var fromProvider = new SqlConfigurationProvider(_session,@from);
            var fromSiteSettings = fromProvider.GetSiteSettings<SiteSettings>();
            var toProvider = new SqlConfigurationProvider(_session, @to);
            var toSiteSettings = toProvider.GetSiteSettings<SiteSettings>();

            var error403 = siteCloneContext.FindNew<Webpage>(fromSiteSettings.Error403PageId);
            if (error403 != null) toSiteSettings.Error403PageId = error403.Id;

            var error404 = siteCloneContext.FindNew<Webpage>(fromSiteSettings.Error404PageId);
            if (error404 != null) toSiteSettings.Error404PageId = error404.Id;

            var error500 = siteCloneContext.FindNew<Webpage>(fromSiteSettings.Error500PageId);
            if (error500 != null) toSiteSettings.Error500PageId = error500.Id;

            var layout = siteCloneContext.FindNew<Layout>(fromSiteSettings.DefaultLayoutId);
            if (layout != null) toSiteSettings.DefaultLayoutId = layout.Id;

            toProvider.SaveSettings(toSiteSettings);
        }
    }
}