using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models.Content;
using MrCMS.Web.Admin.Services.Content;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Web.Admin.Controllers;

public class ContentBlockController : MrCMSAdminController
{
    private readonly IContentBlockAdminService _adminService;

    public ContentBlockController(IContentBlockAdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<ViewResult> Add(int id)
    {
        ViewData["block-options"] = await _adminService.GetContentRowOptions(id);
        return View(await _adminService.GetAddModel(id));
    }

    [HttpPost]
    public async Task<RedirectToActionResult> Add(AddContentBlockModel model)
    {
        await _adminService.AddBlock(model);
        return RedirectToAction("Edit", "ContentVersion", new { id = model.ContentVersionId });
    }

    public async Task<PartialViewResult> Edit(int id)
    {
        ViewData["block"] = await _adminService.GetBlock(id);
        return PartialView(id);
    }

    [HttpPost, ActionName("Edit")]
    public async Task<IActionResult> Edit_POST(int id)
    {
        var model = await _adminService.GetUpdateModel(id);
        if (await TryUpdateModelAsync(model, model.GetType(), ""))
        {
            await _adminService.UpdateBlock(id, model);
        }

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> AddChild(int id)
    {
        await _adminService.AddChild(id);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int id)
    {
        await _adminService.RemoveBlock(id);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Sort(List<ContentBlockSortModel> list)
    {
        await _adminService.SetBlockOrders(list);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ToggleHidden(int id)
    {
        await _adminService.ToggleBlockHidden(id);
        return Ok();
    }
}