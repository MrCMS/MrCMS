using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.WebApi.Api.Controllers
{
    [MrCMSWebApi]
    [Route("api/[controller]")]
    [ApiController]
    public class MrCMSApiController : Controller
    {

    }


    [MrCMSWebApi]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MrCMSAuthApiController : Controller
    {

    }



    [MrCMSAdminWebApi]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MrCMSAdminApiController : MrCMSController
    {

    }
}
