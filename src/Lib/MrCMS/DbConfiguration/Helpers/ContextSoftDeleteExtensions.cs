using Microsoft.AspNetCore.Http;

namespace MrCMS.DbConfiguration.Helpers
{
    public static class ContextSoftDeleteExtensions
    {
        private const string SoftDeleteDisabledKey = "soft-delete-disabled";

        public static void DisableSoftDelete(this HttpContext context)
        {
            context.Items[SoftDeleteDisabledKey] = true;
        }

        public static void EnableSoftDelete(this HttpContext context)
        {
            context.Items.Remove(SoftDeleteDisabledKey);
        }

        public static bool IsSoftDeleteDisabled(this HttpContext context)
        {
            return context.Items.ContainsKey(SoftDeleteDisabledKey);
        }
    }
}