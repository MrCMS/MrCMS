using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models.Content;
using MrCMS.Web.Admin.Services;
using MrCMS.Web.Admin.Services.Content;

namespace MrCMS.Web.Admin.Controllers;

public class ContentVersionController : MrCMSAdminController
{
    private readonly IContentVersionAdminService _adminService;
    private readonly IContentRowAdminService _contentRowAdminService;

    public ContentVersionController(IContentVersionAdminService adminService,
        IContentRowAdminService contentRowAdminService)
    {
        _adminService = adminService;
        _contentRowAdminService = contentRowAdminService;
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

    public async Task<ViewResult> Edit(int id)
    {
        return View(await _adminService.GetEditModel(id));
    }

    public async Task<ViewResult> AddSection(int id)
    {
        ViewData["section-options"] = await _contentRowAdminService.GetContentRowOptions();
        throw new NotImplementedException();
        // return View(await _adminService.GetAddSectionModel(id));
    }
}