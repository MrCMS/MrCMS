using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models.Content;
using MrCMS.Web.Admin.Services.Content;

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
        ViewData["block-options"] = await _adminService.GetContentRowOptions();
        return View(await _adminService.GetAddModel(id));
    }
    
    [HttpPost]
    public async Task<RedirectToActionResult> Add(AddContentBlockModel model)
    {
        await _adminService.AddBlock(model);
        return RedirectToAction("Edit", "ContentVersion", new { id = model.ContentVersionId });
    }
}