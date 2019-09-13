using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Services;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Repositories;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Controllers
{
    public class IS4AdminHomeController : MrCMSAdminController
    {
        private readonly IApiDescriptionGroupCollectionProvider _apiExplorer;
        private readonly ITestRepository _testService;
        public IS4AdminHomeController(IApiDescriptionGroupCollectionProvider apiExplorer, ITestRepository testService)
        {
            _apiExplorer = apiExplorer;
            _testService = testService;
        }

        public IActionResult Index()
        {
          //  return View(_apiExplorer);
           return Content(_testService.GetTestValue());
        }
    }
}
