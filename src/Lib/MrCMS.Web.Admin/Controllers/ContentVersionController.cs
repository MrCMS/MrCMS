using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Infrastructure.Helpers;
using MrCMS.Web.Admin.Models.Content;
using MrCMS.Web.Admin.Services;
using MrCMS.Web.Admin.Services.Content;

namespace MrCMS.Web.Admin.Controllers;

public class ContentVersionController : MrCMSAdminController
{
    private readonly IContentVersionAdminService _adminService;
    private readonly IContentBlockAdminService _contentBlockAdminService;

    public ContentVersionController(IContentVersionAdminService adminService,
        IContentBlockAdminService contentBlockAdminService)
    {
        _adminService = adminService;
        _contentBlockAdminService = contentBlockAdminService;
    }

    public ViewResult AddInitial(int webpageId)
    {
        return View(new AddInitialContentVersionModel { WebpageId = webpageId });
    }

    [HttpPost]
    public async Task<ActionResult> AddInitial(AddInitialContentVersionModel model)
    {
        var version = await _adminService.AddInitialContentVersion(model);

        return RedirectToAction("Edit", "Webpage", new { id = model.WebpageId });
    }

    public async Task<IActionResult> EditLatest(int id)
    {
        var versions = await _adminService.GetVersions(id);

        if (versions.Any(x => x.Status == ContentVersionStatus.Draft))
            return RedirectToAction("Edit", new { versions.First(x => x.Status == ContentVersionStatus.Draft).Id });

        if (!versions.Any(x => x.IsLive))
        {
            var version =
                await _adminService.AddInitialContentVersion(new AddInitialContentVersionModel { WebpageId = id });

            return RedirectToAction("Edit", new { version.Id });
        }
        else
        {
            var liveVersion = versions.First(x => x.IsLive);

            var version = await _adminService.AddDraftBasedOnVersion(liveVersion);

            return RedirectToAction("Edit", new { version.Id });
        }
    }

    public async Task<ViewResult> Edit(int id)
    {
        return View(await _adminService.GetEditModel(id));
    }

    [HttpPost]
    public async Task<RedirectToActionResult> Publish(int id)
    {
        var result = await _adminService.Publish(id);
        if (result.Success)
        {
            TempData.AddSuccessMessage(result.Message);
        }
        else
        {
            TempData.AddErrorMessage(result.Message);
        }

        return result.WebpageId.HasValue
            ? RedirectToAction("Edit", "Webpage", new { id = result.WebpageId })
            : RedirectToAction("Index", "Webpage");
    }

    [HttpPost]
    public async Task<RedirectToActionResult> Delete(int id)
    {
        var result = await _adminService.Delete(id);
        if (result.Success)
        {
            TempData.AddSuccessMessage(result.Message);
        }
        else
        {
            TempData.AddErrorMessage(result.Message);
        }

        return result.WebpageId.HasValue
            ? RedirectToAction("Edit", "Webpage", new { id = result.WebpageId })
            : RedirectToAction("Index", "Webpage");
    }

    public async Task<PartialViewResult> Blocks(int id, Guid? selected, Guid? open)
    {
        ViewData["selected"] = selected;
        ViewData["open"] = open;
        return PartialView(await _adminService.GetEditModel(id));
    }
}