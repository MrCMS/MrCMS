namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Helpers
{
    public static class ViewHelpers
    {
        public static string GetClientName(string clientId, string clientName)
        {
            return $"{clientId} ({clientName})";
        }
    }
}
