using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class ACLController : MrCMSAdminController
    {
        private readonly IACLService _aclService;

        public ACLController(IACLService aclService)
        {
            _aclService = aclService;
        }

        [HttpGet]
        public ViewResult Index()
        {
            return View(_aclService.GetACLModel());
        }

        [HttpPost]
        public RedirectToRouteResult Index([IoCModelBinder(typeof(ACLUpdateModelBinder))] List<ACLUpdateRecord> model)
        {
            _aclService.UpdateACL(model);
            return RedirectToAction("Index");
        }
    }
}