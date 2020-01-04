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
        private readonly AzureProbeSettings _azureProbeSettings;
        private readonly IGlobalRepository<Site> _repository;

        public AzureProbeController(AzureProbeSettings azureProbeSettings, IGlobalRepository<Site> repository)
        {
            _azureProbeSettings = azureProbeSettings;
            _repository = repository;
        }

        public async Task<ActionResult> KeepAlive()
        {
            var item = HttpContext.Request.Query[_azureProbeSettings.Key].ToString();
            if (string.IsNullOrWhiteSpace(item) || item != _azureProbeSettings.Password)
                return new StatusCodeResult(403);

            return new StatusCodeResult(!await _repository.Query().AnyAsync() ? 500 : 200);
        }
    }
}