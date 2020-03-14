using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class GetCurrentLayout : IGetCurrentLayout
    {
        private readonly IRepository<Layout> _repository;
        private readonly IConfigurationProvider _configurationProvider;

        public GetCurrentLayout(IRepository<Layout> repository, IConfigurationProvider configurationProvider)
        {
            _repository = repository;
            _configurationProvider = configurationProvider;
        }

        public async Task<Layout> Get(Webpage webpage)
        {
            if (webpage != null)
            {
                var layout = GetPageTemplateLayout(webpage);
                if (layout != null) return layout;
                layout = await GetTypeDefaultLayout(webpage);
                if (layout != null) return layout;
            }

            return await GetSiteDefault();
        }

        private async Task<Layout> GetTypeDefaultLayout(Webpage webpage)
        {
            var documentMetadata = webpage.GetMetadata();
            var pageDefaultsSettings = await _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            var layoutId = pageDefaultsSettings.GetLayoutId(documentMetadata.Type);
            if (layoutId.HasValue)
            {
                var layout = _repository.GetDataSync(layoutId.Value);
                if (layout != null) return layout;
            }

            return null;
        }

        private static Layout GetPageTemplateLayout(Webpage webpage)
        {
            // todo - I think this will need data access setting up as these properties won't be lazy-loaded
            return webpage.PageTemplate?.Layout;
        }

        private async Task<Layout> GetSiteDefault()
        {
            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            var settingValue = siteSettings.DefaultLayoutId;

            return _repository.GetDataSync<Layout>(settingValue) ??
                   _repository.Readonly().FirstOrDefault();
        }
    }
}