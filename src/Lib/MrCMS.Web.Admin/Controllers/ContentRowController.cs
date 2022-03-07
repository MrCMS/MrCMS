using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models.Content;
using MrCMS.Web.Admin.Services.Content;

namespace MrCMS.Web.Admin.Controllers;

public class ContentRowController : MrCMSAdminController
{
    private readonly IContentBlockAdminService _service;

    public ContentRowController(IContentBlockAdminService service)
    {
        _service = service;
    }
}