using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    [Authorize(AuthenticationSchemes = AuthSchemes)]
    [Route("api/[controller]")]
    [ApiController]
    public class MrCMSAuthApiController : Controller
    {
        private const string AuthSchemes =
            CookieAuthenticationDefaults.AuthenticationScheme + "," +
            JwtBearerDefaults.AuthenticationScheme;

    }



    [MrCMSAdminWebApi]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Administrator")]
    public class MrCMSAdminApiController : Controller
    {
        private const string AuthSchemes =
            CookieAuthenticationDefaults.AuthenticationScheme + "," +
            JwtBearerDefaults.AuthenticationScheme;
    }
}
