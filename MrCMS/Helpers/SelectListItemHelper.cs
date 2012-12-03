using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MrCMS.Helpers
{
    public static class SelectListItemHelper
    {
        public static List<SelectListItem> BuildSelectItemList<T>(this IEnumerable<T> items, Func<T, string> text, Func<T, string> value = null, Func<T, bool> selected = null, string emptyItemText = null)
        {
            return BuildSelectItemList(items, text, value, selected,
                                       !string.IsNullOrWhiteSpace(emptyItemText) ? EmptyItem(emptyItemText) : null);
        }
        public static List<SelectListItem> BuildSelectItemList<T>(this IEnumerable<T> items, Func<T, string> text, Func<T, string> value = null, Func<T, bool> selected = null, SelectListItem emptyItem = null)
        {
            IEnumerable<SelectListItem> selectListItems =
                items.Select(x =>
                             new SelectListItem
                                 {
                                     Text = text.Invoke(x),
                                     Value = value == null ? text.Invoke(x) : value.Invoke(x),
                                     Selected = selected != null && selected.Invoke(x)
                                 });

            return (emptyItem != null
                        ? new List<SelectListItem> {emptyItem}.Union(selectListItems)
                        : selectListItems).ToList();
        }

        public static SelectListItem EmptyItem(string text = null)
        {
            return new SelectListItem { Text = text ?? "None Set", Value = "" };
        }
    }
}