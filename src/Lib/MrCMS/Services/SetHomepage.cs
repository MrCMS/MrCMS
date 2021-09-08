using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services
{
    public class SetHomepage : ISetHomepage
    {
        private readonly ISession _session;
        private readonly IConfigurationProvider _configurationProvider;

        public SetHomepage(ISession session, IConfigurationProvider configurationProvider)
        {
            _session = session;
            _configurationProvider = configurationProvider;
        }

        public async Task<int> Set()
        {
            var homePageId = _session.Query<Webpage>()
                .Where(x => x.Parent == null)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => x.Id)
                .FirstOrDefault();

            if (homePageId > 0)
            {
                var siteSettingsBase = _configurationProvider.GetSiteSettings<SiteSettings>();
                siteSettingsBase.HomePageId = homePageId;
                await _configurationProvider.SaveSettings(siteSettingsBase);
                return homePageId;
            }

            return 0;
        }
    }
}