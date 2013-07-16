using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.ACL;
using MrCMS.Entities.Widget;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class WidgetHelper
    {
        private static IEnumerable<Type> _widgetTypes;

        public static IEnumerable<Type> WidgetTypes
        {
            get { return _widgetTypes ?? (_widgetTypes = TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Widget>()); }
        }

        public static List<SelectListItem> WidgetTypeDropdownItems
        {
            get
            {
                return WidgetTypes.BuildSelectItemList(type => type.Name.BreakUpString(), type => type.Name,
                                                       emptyItemText: null);
            }
        }

        public static List<SelectListItem> AllowedWidgetTypeDropdownItems
        {
            get
            {
                return WidgetTypes.Where(
                    type =>
                    CurrentRequestData.CurrentUser.CanAccess<TypeACLRule>(TypeACLRule.Add, type.FullName))
                                  .BuildSelectItemList(type => type.Name.BreakUpString(), type => type.Name,
                                                       emptyItemText: null);
            }
        }

        public static Type GetTypeByName(string typeName)
        {
            return WidgetTypes.FirstOrDefault(x => x.Name == typeName);
        }
    }
}