using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services.Content;

namespace MrCMS.Web.Admin.Controllers;

public class BlockItemController : MrCMSAdminController
{
    private readonly IContentBlockItemAdminService _adminService;

    public BlockItemController(IContentBlockItemAdminService adminService)
    {
        _adminService = adminService;
    }
    public async Task<ActionResult> Edit(int id, Guid itemId)
    {
        ViewData["id"] = id;
        ViewData["item"] = await _adminService.GetBlockItem(id, itemId);
        return PartialView();
    }
    
    [HttpPost, ActionName("Edit")]
    public async Task<ActionResult> Edit_POST(int id, Guid itemId)
    {
        var model = await _adminService.GetUpdateModel(id,itemId);
        if (await TryUpdateModelAsync(model, model.GetType(), ""))
        {
            await _adminService.UpdateBlockItem(id,itemId, model);
        }

        return Ok();
    }
}