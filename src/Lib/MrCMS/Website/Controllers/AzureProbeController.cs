using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Website.Controllers
{
    public class AzureProbeController : MrCMSUIController
    {
        private readonly ISystemConfigurationProvider _configurationProvider;
        private readonly IGlobalRepository<Site> _repository;

        public AzureProbeController(ISystemConfigurationProvider configurationProvider, IGlobalRepository<Site> repository)
        {
            _configurationProvider = configurationProvider;
            _repository = repository;
        }

        public async Task<ActionResult> KeepAlive()
        {
            var azureProbeSettings = await _configurationProvider.GetSystemSettings<AzureProbeSettings>();
            var item = HttpContext.Request.Query[azureProbeSettings.Key].ToString();
            if (string.IsNullOrWhiteSpace(item) || item != azureProbeSettings.Password)
                return new StatusCodeResult(403);

            return new StatusCodeResult(!await _repository.Query().AnyAsync() ? 500 : 200);
        }
    }
}