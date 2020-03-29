using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Resources;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public static class StringResourceExtensions
    {
        public static IEnumerable<StringResource> GetResourcesByKeyAndValue(
            this IEnumerable<StringResource> resourcesForQuery, StringResourceSearchQuery searchQuery)
        {
            IEnumerable<StringResource> resources = resourcesForQuery;

            if (!string.IsNullOrWhiteSpace(searchQuery.Key))
            {
                resources =
                    resources.Where(
                        resource => resource.Key.Contains(searchQuery.Key, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(searchQuery.Value))
            {
                resources =
                    resources.Where(
                        resource => resource.Value.Contains(searchQuery.Value, StringComparison.OrdinalIgnoreCase));
            }
            return resources;
        }

        public static string GetDisplayKey(string key)
        {
            if (key == null || key.LastIndexOf(".", StringComparison.Ordinal) == -1)
            {
                return key;
            }
            var typeName = key.Substring(0, key.LastIndexOf(".", StringComparison.Ordinal));
            var type = TypeHelper.GetTypeByName(typeName);
            if (type != null)
            {
                return type.Name.BreakUpString() + " - " +
                       key.Substring(key.LastIndexOf(".", StringComparison.Ordinal) + 1).BreakUpString();
            }
            return key;
        }
    }
}