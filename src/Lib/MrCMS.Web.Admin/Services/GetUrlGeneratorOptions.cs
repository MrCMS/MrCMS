using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Web.Admin.Services
{
    public class GetUrlGeneratorOptions : IGetUrlGeneratorOptions
    {
        public List<SelectListItem> Get(Type type, Type currentGeneratorType = null)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            if (type != null)
            {
                Type defaultGenerator = typeof(DefaultWebpageUrlGenerator);
                selectListItems.Add(new SelectListItem { Text = "Default", Value = defaultGenerator.FullName });
                selectListItems.AddRange(
                    TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(WebpageUrlGenerator<>).MakeGenericType(type)).Select(
                        generatorType =>
                            new SelectListItem
                            {
                                Text = generatorType.Name.BreakUpString(),
                                Value = generatorType.FullName,
                                Selected = generatorType == currentGeneratorType
                            }));
            }
            return selectListItems;
        }
    }
}