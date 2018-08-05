using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Resources;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public static class StringResourceSearchExtensions
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
    }
}