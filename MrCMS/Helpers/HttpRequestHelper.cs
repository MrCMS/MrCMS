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

            var connectingIp = request.Headers["CF-CONNECTING-IP"];
            if (!string.IsNullOrWhiteSpace(connectingIp))
                return connectingIp;

            string ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrWhiteSpace(ipAddress))
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