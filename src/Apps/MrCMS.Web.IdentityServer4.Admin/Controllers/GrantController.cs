using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Grant;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Services;
using MrCMS.Web.Apps.IdentityServer4.Admin.Helpers;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Controllers
{
    public class GrantController : MrCMSAdminController
    {
        private readonly IPersistedGrantService _persistedGrantService;
        private readonly IStringLocalizer<GrantController> _localizer;
        private readonly ILogger<GrantController> _logger;
        private readonly IStringResourceProvider _stringLocalizer;

        public GrantController(IPersistedGrantService persistedGrantService,
            ILogger<GrantController> logger,
            IStringLocalizer<GrantController> localizer,IStringResourceProvider stringResourceProvider) 
        {
            _persistedGrantService = persistedGrantService;
            _localizer = localizer;
            _logger = logger;
            _stringLocalizer = stringResourceProvider;
        }

        [HttpGet]
        public async Task<IActionResult> PersistedGrants(int? page, string search)
        {
            ViewBag.Search = search;
            var persistedGrants = await _persistedGrantService.GetPersistedGrantsByUsersAsync(search, page ?? 1);

            return View(persistedGrants);
        }

        [HttpGet]
        public async Task<IActionResult> PersistedGrantDelete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var grant = await _persistedGrantService.GetPersistedGrantAsync(UrlHelpers.QueryStringUnSafeHash(id));
            if (grant == null) return NotFound();

            return View(grant);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersistedGrantDelete(PersistedGrantDto grant)
        {
            await _persistedGrantService.DeletePersistedGrantAsync(grant.Key);

            // SuccessNotification(_localizer["SuccessPersistedGrantDelete"], _localizer["SuccessTitle"]);
            TempData.SuccessMessages().Add(string.Format(_stringLocalizer.GetValue("SuccessPersistedGrantDelete", "Client claim is successfully deleted!")));
            return RedirectToAction(nameof(PersistedGrants));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersistedGrantsDelete(PersistedGrantsDto grants)
        {
            await _persistedGrantService.DeletePersistedGrantsAsync(grants.SubjectId);

            /// SuccessNotification(_localizer["SuccessPersistedGrantsDelete"], _localizer["SuccessTitle"]);
            TempData.SuccessMessages().Add(string.Format(_stringLocalizer.GetValue("SuccessPersistedGrantsDelete", "Client claim is successfully deleted!")));
            return RedirectToAction(nameof(PersistedGrants));
        }


        [HttpGet]
        public async Task<IActionResult> PersistedGrant(string id, int? page)
        {
            var persistedGrants = await _persistedGrantService.GetPersistedGrantsByUserAsync(id, page ?? 1);
            persistedGrants.SubjectId = id;

            return View(persistedGrants);
        }
    }
}