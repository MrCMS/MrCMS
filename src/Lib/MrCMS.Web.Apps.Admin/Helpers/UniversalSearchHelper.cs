using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Helpers;
using MrCMS.Search;

namespace MrCMS.Web.Apps.Admin.Helpers
{
    public static class UniversalSearchHelper
    {
        public static List<SelectListItem> GetOptions()
        {
            return TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(GetUniversalSearchItemBase<>))
                .Select(
                    x =>
                        x.GetBaseTypes()
                            .First(y => y.GetGenericTypeDefinition() == typeof(GetUniversalSearchItemBase<>))
                            .GetGenericArguments()[0])
                .OrderBy(x => x.Name)
                .BuildSelectItemList(x => x.Name.BreakUpString(), x => x.FullName, emptyItemText: "All");
        }
    }
}