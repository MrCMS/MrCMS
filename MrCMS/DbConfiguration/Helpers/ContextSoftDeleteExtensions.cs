using System.Web;

namespace MrCMS.DbConfiguration.Helpers
{
    public static class ContextSoftDeleteExtensions
    {
        private const string SoftDeleteDisabledKey = "soft-delete-disabled";

        public static void DisableSoftDelete(this HttpContextBase context)
        {
            context.Items[SoftDeleteDisabledKey] = true;
        }
        public static void EnableSoftDelete(this HttpContextBase context)
        {
            context.Items.Remove(SoftDeleteDisabledKey);
        }

        public static bool IsSoftDeleteDisabled(this HttpContextBase context)
        {
            return context.Items.Contains(SoftDeleteDisabledKey);
        }
    }
}