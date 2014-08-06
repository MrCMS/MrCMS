using System.Web;

namespace MrCMS.Helpers
{
    public static class HttpRequestHelper
    {
        public static string GetCurrentIP(this HttpContextBase contextBase)
        {
            string ipAddress = contextBase.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    if (addresses[0].Contains(":"))
                        return addresses[0].Split(':')[0];
                    return addresses[0];
                }
            }

            return contextBase.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}