using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Web.Admin.Services
{
    public interface IGetUrlGeneratorOptions
    {
        List<SelectListItem> Get(Type type, Type currentGeneratorType = null);
    }
}