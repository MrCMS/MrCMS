using System.Web;

namespace MrCMS.Helpers
{
    public static class HttpRequestHelper
    {
        public static string GetCurrentIP(this HttpContextBase contextBase)
        {
            return contextBase.Request.GetCurrentIP();
        }

        public static string GetCurrentIP(this HttpRequestBase request)
        {
            if (request == null)
                return string.Empty;
            string ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

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

            return request.ServerVariables["REMOTE_ADDR"];
        }
    }
}