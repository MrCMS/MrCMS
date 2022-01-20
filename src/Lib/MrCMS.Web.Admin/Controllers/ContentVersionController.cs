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

    public async Task<ViewResult> Edit(int id)
    {
        return View(await _adminService.GetEditModel(id));
    }
    

    public async Task<PartialViewResult> Blocks(int id, Guid? selected, Guid? open)
    {
        ViewData["selected"] = selected;
        ViewData["open"] = open;
        return PartialView(await _adminService.GetEditModel(id));
    }
}