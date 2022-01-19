using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models.Content;
using MrCMS.Web.Admin.Services.Content;

namespace MrCMS.Web.Admin.Controllers;

public class ContentRowController : MrCMSAdminController
{
    private readonly IContentRowAdminService _service;

    public ContentRowController(IContentRowAdminService service)
    {
        _service = service;
    }

    public async Task<IReadOnlyList<ContentRowOption>> Options()
    {
        return await _service.GetContentRowOptions();
    }
}