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
        private static readonly HashSet<Type> GeneratorTypes =
            TypeHelper.GetAllConcreteTypesAssignableFromGeneric(typeof(WebpageUrlGenerator<>));

        public List<SelectListItem> Get(Type type, Type currentGeneratorType = null)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            if (type != null)
            {
                Type defaultGenerator = typeof(DefaultWebpageUrlGenerator);
                selectListItems.Add(new SelectListItem {Text = "Default", Value = defaultGenerator.FullName});
                selectListItems.AddRange(GeneratorTypes
                    .FindAll(x => typeof(WebpageUrlGenerator<>).MakeGenericType(type).IsAssignableFrom(x)).Select(
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