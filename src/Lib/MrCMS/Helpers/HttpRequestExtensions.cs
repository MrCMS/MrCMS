using System.Net;
using Microsoft.AspNetCore.Http;

namespace MrCMS.Helpers
{
    public static class HttpRequestExtensions
    {
        public static bool IsLocal(this HttpRequest req)
        {
            var connection = req.HttpContext.Connection;
            if (connection.RemoteIpAddress != null)
            {
                return connection.LocalIpAddress != null
                    ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                    : IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            // for in memory TestServer or when dealing with default connection info
            return connection.RemoteIpAddress == null && connection.LocalIpAddress == null;
        }

        public static string Referer(this HttpRequest req)
        {
            return req.Headers["Referer"];
        }

        //public static string GetCurrentIP(this HttpContext contextBase)
        //{
        //    return contextBase.Request.GetCurrentIP();
        //}

        public static string UserAgent(this HttpRequest request)
        {
            return request.Headers["User-Agent"].ToString();
        }

        public static string GetCurrentIP(this HttpRequest request)
        {
            if (request == null)
                return string.Empty;

            var connectingIp = request.Headers["CF-CONNECTING-IP"];
            if (!string.IsNullOrWhiteSpace(connectingIp))
                return connectingIp;

            string ipAddress = request.Headers["X-Forwarded-For"];

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

            return request.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }

}