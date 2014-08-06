using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IGetUrlGeneratorOptions
    {
        List<SelectListItem> Get(Type type, Type currentGeneratorType = null);
    }
}